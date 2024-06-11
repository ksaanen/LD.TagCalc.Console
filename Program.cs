using LD.Tag;
using LegoDimensions.Tag;

byte[] _uid = [];
ushort _charid = 00;

string _choice = "";

while (_uid.Length != 7)
{
    Console.WriteLine("Enter Tag UID:");
    string str = Console.ReadLine();
    if (str.Length != 14)
    {
        Console.WriteLine("String not long enough:");
        str = Console.ReadLine();
    }
    else
    {
        _uid = StringToByteArray(str);
    }
}


while (_charid < 01 || _charid > 80)
{
    Console.WriteLine("Enter Character ID you wish to generate:");

    for (int i = 0; i < Character.Characters.Count; i++)
    {
        Console.WriteLine(Character.Characters[i].Id.ToString() + ": " + Character.Characters[i].Name.ToString());
    }
    _charid = ushort.Parse(Console.ReadLine());
}

static byte[] StringToByteArray(string hex)
{
    return Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
}

byte[] keys = LegoTag.EncrypCharactertId(_uid, _charid);

static string getCharacterById(ushort id)
{
    string result = Character.Characters.Find(x => x.Id == id).Name;
    if (result == null)
    {
        return "null";
    }
    return result;
}
Console.Clear();
Console.WriteLine($"You've picked: {getCharacterById(_charid)}");
Console.WriteLine($"Write to page 24: {keys[0].ToString("X2")} {keys[1].ToString("X2")} {keys[2].ToString("X2")} {keys[3].ToString("X2")}");
Console.WriteLine($"Write to page 25: {keys[4].ToString("X2")} {keys[5].ToString("X2")} {keys[6].ToString("X2")} {keys[7].ToString("X2")}");
