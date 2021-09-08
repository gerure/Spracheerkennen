using System;
/*
Vielen Dank für die interessante Aufgabe.
Als erstes habe ich versucht ob in dem String individuelle (nicht lateinische) Buchstaben zu finden sind.
Deutsch, Spanish oder Französisch könnte man so eindeutig erkennen, wenn solche Buchstaben vorhanden sind.
Wenn hier keine eindeutige Ergebnisse geliefert wird, wird nach Wörtern gesucht.

Dafür habe ich einpaar häufig verwendete Wörte der jeweiligen Sprache sozusagen
als Wörterbuch benutzt. Sonst könnte man richtige Wörterbuchsuche benutzen. 
(was aber nach meiner Meinung unpraktisch wäre, da für jedes Wort im String 4 Wörterbücher durchsucht werden muss.)

Aber die Liste der häufig verwendeten (aber auch eindeutigen) Wörter könnte/sollte man für eine 
richtige Anwendung erweitern.
Bei der Wortsuche wird hier genau gezählt von den 4 Wortlisten wieviele Wörter genau von der jeweiligen Sprache
auftaucht. Dann wird analysiert welche Sprache es sein könnte. 

Bei größeren Texteingaben könnte man lange Wörter (z.B. mit mehr als 6 Buchstaben) schon vorher 
beim Parsen rausnehmen, damit der Zeitaufwand akzeptabel bleibt.
 */
namespace Spracheerkennen
{
    class Spracherkennung
    {
        //Erst werden nach eindeutigen Buchstaben gesucht, falls erfolglos dann beginnt die Wortsuche.
        //English wird ausgelassen, da es keine individuelle (bzw. nicht lateinische) Buchstaben gibt.
        char[] deutscheBuchstaben = { 'ä', 'ö', 'ü', 'ß' };
        char[] franzoesischeBuchstaben = { 'â', 'à', 'æ', 'ê', 'è', 'ë', 'î', 
                                            'ï', 'ô', 'ò', 'œ', 'û', 'ù', 'ç'};
        char[] spanischeBuchstaben = { 'ñ', '¿', 'á', 'í', 'ó', 'ú' };

        //man könnte hier eine Wörterbuchsuche benutzen, wenn man Wörterbücher zur Verfügung hätte. (z.B. in Textformat )
        //Da es bei der Aufgabe vermutlich mehr um meinen Lösungsansatz geht, habe ich einfach
        //einpaar häufig benutzte Wörter genommen ;-)
        string[] englishWoerter = { "the", "at", "there", "some", "of", "be", "use", "her",
                                    "than", "and", "this", "which", "like", "been", "who",
                                    "or", "is", "one", "by", "into", "if", "its", "now", "a",
                                    "as", "with", "when", "no", "get", "his", "said", "part",
                                    "way", "may", "are", "his", "other", "true", "go", "come"};
        string[] deutschWoerter = { "ich", "du", "er", "sie", "es", "wir", "ihr","was", "warum", 
                                    "wo", "wie","wann", "nicht", "aber", "das", "alle", "oder",
                                    "und", "dann", "weil", "bitte", "komm", "wer", "neu", "alt",
                                    "gut", "schlecht", "richtig", "kurz", "lang", "dort", "hallo"};
        string[] spanisheWoerter = { "yo", "tú", "él", "ella", "eso", "nosotros", "ustedes", "ellos",
                                     "qué", "quién", "dónde", "por", "cómo", "cuál", "todos", "ese",
                                     "o", "y", "este", "si", "cero", "uno", "dos", "tres", "cuatro",
                                     "cinco", "seis", "nuevo", "viejo", "pocos", "muchos", "bueno", "corto"};
        string[] franzoesischeWoerter = {"oui", "s’il", "plaît", "vous", "merci", "pardon", "d'accord",
                                        "parle", "pas", "au", "peu", "vos", "où", "suis", "avez", " âge",
                                        "salut", "bien", "coucou", "vous", "faisait", "gauche", "loin",
                                        "marché", "cher", "l‘étage", "un", "deux", "trois", "quatre", "cinq"};

        // 0.5 -> wenn mindestens 50% von den gefundenen Wörtern von einer Sprache ist,
        // dann gehe ich davon aus, dass dies die Sprache ist.
        // wenn man sicherer/strengerer sein will erhöht man diesen Wert bis 1 (1 = 100%)
        double genauigkeitBeiWortsuche = (double)0.5;

        //ich habe hier den String ohne Leerzeichen, Punkte und Komma in Wörtern getrennt,
        //damit ich Wort für Wort genau vergleichen kann.
        //sonst mit "eingabeString.Contains(wort)" würde man falsche Ergebnisse bekommen.
        //z.B. bei dem Wort "woher" würde man das englische Wort "her" finden und mitzählen.
        string[] woerter;
        char[] buchstaben;
        int anzahlDeutscheBuchstaben, anzahlFranzoesischeBuchstaben, anzahlSpanischeBuchstaben;
        int anzahlEnglischeWoerter, anzahlDeutscheWoerter, anzahlFranzoesischeWoerter, anzahlSpanischeWoerter;

        private string ErkenneSprache(String eingabe)
        {
            if (eingabe == "") return "Eingabe ist leer";
            ParseBuchstabenUndWoerter(eingabe);
            bool isText = false;

            //Hier wird geprüft, ob überhaupt Buchstaben vorhanden sind.
            foreach (char c in this.buchstaben)
            {   //bei der ersten gefundenen Buchstabe, wird die Schleife abgebrochen.
                if (Char.IsLetter(c)) { isText = true; break;} 
            }
            if (!isText) return "Eingabe enthält keine Buchstaben bzw. Text";
            int gefunden = SucheBuchstaben(eingabe);
            if (gefunden >0) //Es sind Deutsche, Französische oder Spanische Buchstaben gefunden worden.
            {
                if (gefunden == anzahlDeutscheBuchstaben) return "Deutsch";
                if (gefunden == anzahlFranzoesischeBuchstaben) return "Französisch";
                if (gefunden == anzahlSpanischeBuchstaben) return "Spanisch";
                //hier kommt man an, wenn die Buchstabensuche mehrere Sprachen identifiziert hat.
            }
            gefunden = SucheWoerter(); 
            Console.WriteLine("insgesamt " + gefunden + " Wörter gefunden");
            //gefunden == anzahlEnglischeWoerter -> Sprache eindeutig identifiziert
            //(double)anzahlEnglischeWoerter /gefunden >= genauigkeitBeiWortsuche -> nicht ganz eindeutig aber
            //entsprechend der angegebenen Genauigkeit identifiziert.
            if (gefunden > 0)
            { 
                if (gefunden == anzahlEnglischeWoerter 
                    || (double)anzahlEnglischeWoerter /gefunden >= genauigkeitBeiWortsuche) return "English";
                if (gefunden == anzahlDeutscheWoerter
                    || (double)anzahlDeutscheWoerter /gefunden >= genauigkeitBeiWortsuche) return "Deutsch";
                if (gefunden == anzahlFranzoesischeWoerter
                    || (double)anzahlFranzoesischeWoerter /gefunden >= genauigkeitBeiWortsuche) return "Französisch";
                if (gefunden == anzahlSpanischeWoerter
                    || (double)anzahlSpanischeWoerter/gefunden >= genauigkeitBeiWortsuche) return "Spanisch";
                //hier kommt man an, wenn Wörter zwar gefunden wurden aber die Sprache nicht eindeutig ist.
            }
            return "nicht eindeutig";
            
        }

        //hier werden eindeutig identifizierbare "nicht lateinische" Buchstaben gesucht und gezählt.
        //Rückgabe = die Gesamtanzahl der gefundenen Buchstaben.
        private int SucheBuchstaben(String eingabe)
        {
            foreach (char a in this.deutscheBuchstaben)
            {
                if (eingabe.Contains(a) == true) this.anzahlDeutscheBuchstaben++;
            }
            foreach (char a in this.spanischeBuchstaben)
            {
                if (eingabe.Contains(a) == true) this.anzahlSpanischeBuchstaben++;
            }
            foreach (char a in this.franzoesischeBuchstaben)
            {
                if (eingabe.Contains(a) == true) this.anzahlFranzoesischeBuchstaben++;
            }

            return anzahlDeutscheBuchstaben
                + anzahlFranzoesischeBuchstaben
                + anzahlSpanischeBuchstaben;
        }
        //Die eingegebenen Wörter werden mit den Wörtern aus den 4 Listen verglichen und die Treffer werden mitgezählt.
        // Rückgabe = die Gesamtanzahl der gefundenen Wörter
        private int SucheWoerter()
        {
            foreach (string wort in woerter)
            {
                foreach (string s in englishWoerter)
                {
                    if (wort == s) anzahlEnglischeWoerter++;
                }
                foreach (string s in deutschWoerter)
                {
                    if (wort == s) anzahlDeutscheWoerter++;
                }
                foreach (string s in spanisheWoerter)
                {
                    if (wort == s) anzahlSpanischeWoerter++;
                }
                foreach (string s in franzoesischeWoerter)
                {
                    if (wort == s) anzahlFranzoesischeWoerter++;
                }
            }
            Console.WriteLine(anzahlDeutscheWoerter + " x deutsche " 
                + anzahlEnglischeWoerter + " x englishe "
                + anzahlFranzoesischeWoerter + " x französische "
                + anzahlSpanischeWoerter + " x spanishe Wörter gefunden");
            return anzahlDeutscheWoerter
                + anzahlEnglischeWoerter
                + anzahlFranzoesischeWoerter
                + anzahlSpanischeWoerter;
        }
        private void ParseBuchstabenUndWoerter(string eingabe)
        {
            //Um die Buchstaben zu parsen werden erstmal die Leerzeichen entfernt
            woerter = eingabe.Split(' ',',','.');
            string eingabeOhneLeerzeichen = eingabe.Replace(" ", "");
            this.buchstaben = eingabeOhneLeerzeichen.Replace(" ", "").ToCharArray(0, eingabeOhneLeerzeichen.Length);
        }

        static void Main(string[] args)
        {
            Spracherkennung s = new Spracherkennung();
            Console.WriteLine("Bitte um Ihre Eingabe: ");
            String eingabe = Console.ReadLine();
            Console.WriteLine(s.ErkenneSprache(eingabe.ToLower())); //alle Buchstaben "kleinmachen" für die Suche
        }

    }
}
