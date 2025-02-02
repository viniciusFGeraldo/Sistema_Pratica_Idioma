using System.Speech.Synthesis;
using System.Text.Json.Serialization;
using AppPraticaIdioma.Models;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>();
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options => options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddCors(options => 
    options.AddPolicy("Acesso Total", 
        configs => configs
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod())
);


var app = builder.Build();

app.MapGet("/", () => "Hello World!");


app.MapPost("/phrase/cadastrar", ([FromBody] Phrase phrase, [FromServices] AppDbContext ctx) =>
{
    var existingPhrase = ctx.Phrases.FirstOrDefault(p => p.NativeLanguage == phrase.NativeLanguage);
    if (existingPhrase is not null)
    {
        return Results.BadRequest("Essa frase já existe!");
    }

    // Adiciona a frase no banco de dados
    ctx.Phrases.Add(phrase);
    ctx.SaveChanges();

    // Retorna o status 201 (Created) com o ID gerado
    return Results.Created($"/phrase/{phrase.Id}", phrase);
});

//lista todas as frases
app.MapGet("/phrase/listar", ([FromServices] AppDbContext ctx) =>{
    if (ctx.Phrases.Any()){
        return Results.Ok(ctx.Phrases.ToList());
    }

    return Results.NotFound("Tabela vazia!");
});

//pega uma frase pelo id
app.MapGet("/phrase/buscar/{id}", ([FromRoute] int id, [FromServices] AppDbContext ctx) =>{
    //Expressão lambda em c#
    Phrase? frase = ctx.Phrases.FirstOrDefault(x => x.Id == id);

    if (frase is null){
        return Results.NotFound("Frase não encontrado!");
    }

    return Results.Ok(frase);
});

//alterar frase
app.MapPut("/phrase/alterar/{id}", ([FromRoute] int id, [FromBody] Phrase fraseAlterada, [FromServices] AppDbContext ctx) => {
    Phrase? frase = ctx.Phrases.Find(id);

    if (frase is null){
        return Results.NotFound("Frase não encontrada!");
    }

    frase.NativeLanguage = fraseAlterada.NativeLanguage;
    frase.ForeignLanguage = fraseAlterada.ForeignLanguage;
    

    ctx.Phrases.Update(frase);
    ctx.SaveChanges();

    return Results.Ok("Frase alterada com sucesso!");
});

//deletar frase
app.MapDelete("/phrase/deletar/{id}", ([FromRoute] int id, [FromServices] AppDbContext ctx) =>{
    Phrase? frase = ctx.Phrases.Find(id);

    if (frase is null){
        return Results.NotFound("Frase não encontrada!");
    }

    ctx.Phrases.Remove(frase);
    ctx.SaveChanges();
    return Results.Ok(ctx.Phrases.ToList());
});


app.MapPost("/phrases/speak/{id}", async ([FromServices] AppDbContext ctx, [FromRoute] int id, [FromQuery] bool speakInEnglish) =>
{
    var phrase = await ctx.Phrases.FindAsync(id);
    if (phrase == null) return Results.NotFound("Frase não encontrada.");

    if (!OperatingSystem.IsWindows())
    {
        return Results.BadRequest("Recurso de fala só é suportado no Windows.");
    }

    using var falar = new SpeechSynthesizer();
    using var synth = new SpeechSynthesizer();

    string selectedVoice = null;

    foreach (InstalledVoice voice in synth.GetInstalledVoices())
    {
        if (speakInEnglish && voice.VoiceInfo.Culture.Name.StartsWith("en"))
        {
            selectedVoice = voice.VoiceInfo.Name;
            break;
        }
        else if (!speakInEnglish && voice.VoiceInfo.Culture.Name.Contains("pt-BR"))
        {
            selectedVoice = voice.VoiceInfo.Name;
            break;
        }
    }

    if (selectedVoice != null)
    {
        falar.SelectVoice(selectedVoice);
    }
    else
    {
        falar.SelectVoiceByHints(speakInEnglish ? VoiceGender.Neutral : VoiceGender.Female, VoiceAge.Adult);
    }

    falar.Rate = 0;

    // Reproduzir a frase conforme o idioma escolhido
    if (speakInEnglish)
    {
        falar.Speak($"In English: {phrase.ForeignLanguage}");
    }
    else
    {
        falar.Speak($"Em português: {phrase.NativeLanguage}");
    }

    return Results.Ok("Frase reproduzida.");
});

app.UseCors("Acesso Total");
app.Run();
