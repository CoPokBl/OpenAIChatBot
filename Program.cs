using System.Text;
using System.Text.Json;
using GeneralPurposeLib;

if (!File.Exists("OpenAIToken.txt")) {
    Console.WriteLine("OpenAI Token not found. Please create a file called OpenAIToken.txt and put your OpenAI Token in it.");
    return 1;
}
string openAiToken = File.ReadAllText("OpenAIToken.txt");

string GetResponse(string prompt) {
    HttpClient client = new();
    client.BaseAddress = new Uri("https://api.openai.com/v1/completions");
    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + openAiToken);
    string promptContent = prompt + "\nBot: ";
    StringContent content = new(
        new {
            model = "text-davinci-002",
            prompt = promptContent,
            temperature = 0.7,
            max_tokens = 100,
            top_p = 1,
            frequency_penalty = 0,
            presence_penalty = 0
        }.ToJson(), 
        Encoding.UTF8, 
        "application/json");
    
    HttpResponseMessage response = client.PostAsync(client.BaseAddress, content).Result;
    string responseString = response.Content.ReadAsStringAsync().Result;
    JsonDocument document = JsonDocument.Parse(responseString);
    JsonElement root = document.RootElement;
    JsonElement choices = root.GetProperty("choices");
    JsonElement choice = choices[0];
    JsonElement text = choice.GetProperty("text");
    return (text.GetString() ?? "").Replace("\n", "");
}

Console.Title = "Chat Bot";
Console.WriteLine("Hello!");

if (!Directory.Exists("Personalities")) {
    Console.WriteLine("Personalities folder not found!");
    return 1;
}

string[] personalities = Directory.GetFiles("Personalities");
string[] personalitiesWithoutExtensions = new string[personalities.Length];
for (int i = 0; i < personalities.Length; i++) {
    personalitiesWithoutExtensions[i] = Path.GetFileNameWithoutExtension(personalities[i]);
}

Console.WriteLine("Select a personality:");
for (int i = 1; i < personalitiesWithoutExtensions.Length+1; i++) {
    Console.WriteLine(i + ": " + personalitiesWithoutExtensions[i-1]);
}

int personality = 0;
while (personality < 1 || personality > personalitiesWithoutExtensions.Length) {
    Console.Write("Personality: ");
    int.TryParse(Console.ReadLine(), out personality);  // If it fails, it will be 0
}

Console.WriteLine("Loading personality...");
string personalityFile = File.ReadAllText(personalities[personality-1]);

// Talk Loop
while (true) {
    Console.Write("You: ");
    string input = Console.ReadLine() ?? "";
    Console.WriteLine("Bot: " + GetResponse(personalityFile + input));
}

return 0;