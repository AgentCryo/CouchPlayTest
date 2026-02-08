namespace CouchPlayTest.Drawing.Font;

public struct Font(string fontFilePath)
{
    public string FontFilePath = fontFilePath;
    public (int[] dimensions, List<byte[]> characterSet) FontData = FontUtility.GetCharacterSet(fontFilePath);
}