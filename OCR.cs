using System;
using System.Collections.Generic;
using System.Drawing;
using Tesseract;


public static class OCR
{
    public static string ocr(string filepath)
    {
        string text = "";
        try
        {
            // Path to the tessdata folder (language data files)
            string tessDataPath = @"C:\MySys\AutoKnowns\tessdata";

            // Initialize Tesseract Engine (English language)
            using (var engine = new TesseractEngine(tessDataPath, "jpn", EngineMode.Default))
            {
                engine.SetVariable("user_defined_dpi", "300");
                // Load the image for OCR
                using (var img = Pix.LoadFromFile(filepath))
                {
                    using (var page = engine.Process(img))
                    {
                        var a = page.GetText();
                        if(a != null)
                            text = page.GetText().Replace("\n", "");
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }
        return text;
    }
}