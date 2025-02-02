namespace AppPraticaIdioma.Models;

public class Phrase
{
    public Phrase(){}

    public Phrase(string nativeLanguage, string foreignLanguage){
        NativeLanguage = nativeLanguage;
        ForeignLanguage = foreignLanguage;
    }
    public int Id { get; set; }
    public string NativeLanguage { get; set; } = string.Empty;
    public string ForeignLanguage { get; set; } = string.Empty;
}