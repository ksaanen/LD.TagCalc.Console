using LegoDimensions.Tag;

class Program
{

    private static byte[] _uid = [];
    private static ushort _id = ushort.MinValue;
    private static byte[] _keys = [];
    private static byte[] _pwd = [];

    public static void Main(string[] args)
    {
        HandleArguments(args);

        if (args != null && args.Length != 0)
        {
            return;
        }

        System.Console.WriteLine("Enter the NTAG213 UID:");
        System.Console.WriteLine("First 3 hex values of address 0x00 and all 4 hex values of address 0x01");
        System.Console.WriteLine("Example:");
        System.Console.WriteLine("0x00: FF 01 23 45");
        System.Console.WriteLine("0x01: 67 89 0A BC");
        System.Console.WriteLine("UID: FF012367890ABC");

        do
        {
            string? str = Console.ReadLine();
            if (str == null | !IsHex(str))
            {
                Console.WriteLine("This is not a valid UID");
            }

            if (str.Length != 14)
            {
                System.Console.WriteLine("UID should be 14 chars (7 hex values) long.");
                str = System.Console.ReadLine();
            }
            else
            {
                _uid = StringToByteArray(str);
            }
        }
        while (_uid.Length != 7);


        do
        {
            System.Console.WriteLine("Enter the character or vehicle ID you wish to generate:");
            _id = ushort.Parse(System.Console.ReadLine());
        }
        while (_id == ushort.MinValue);

        static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                            .Where(x => x % 2 == 0)
                            .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                            .ToArray();
        }

        _pwd = LegoTag.GenerateCardPassword(_uid);
        if (_id.ToString().Length < 4)
        {
            _keys = LegoTag.EncryptCharacterId(_uid, _id);
            PrintCharacterResult(_keys, _pwd);
        }
        if (_id.ToString().Length == 4)
        {
            _keys = LegoTag.EncryptVehicleId(_id);
            PrintVehicleResult(_keys, _pwd);
        }
    }

    private static void HandleArguments(string[] args)
    {
        if (args != null)
        {
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-h":
                    case "--help":
                        PrintHelp();
                        break;
                    case "-c":
                    case "--list-characters":
                        PrintCharacters();
                        break;
                    case "-v":
                    case "--list-vehicles":
                        PrintVehicles();
                        break;
                    default:
                        PrintDefault();
                        break;
                }
            }
        }
    }

    private static void PrintDefault()
    {
        System.Console.WriteLine("The parameter you entered is not supported. Type -h or --help for more information.");
    }

    private static void PrintCharacters()
    {
        for (int i = 0; i < Character.Characters.Count; i++)
        {
            System.Console.WriteLine(Character.Characters[i].Id.ToString() + ": " + Character.Characters[i].Name.ToString());
        }
    }

    private static void PrintVehicles()
    {
        for (int i = 0; i < Vehicle.Vehicles.Count; i++)
        {
            System.Console.WriteLine(Vehicle.Vehicles[i].Id.ToString() + ": " + Vehicle.Vehicles[i].Name.ToString());
        }
    }

    private static void PrintHelp()
    {
        System.Console.WriteLine("Type -c or --list-characters to list all character options.");
        System.Console.WriteLine("Type -v or --list-vehicles to list all vehicle options.");
    }

    private static void PrintVehicleResult(byte[] keys, byte[] pwd)
    {
        string name = getVehicleById(_id);

        System.Console.Clear();
        System.Console.WriteLine($"You've picked: {name}");
        System.Console.WriteLine($"0x24: {keys[0].ToString("X2")} {keys[1].ToString("X2")} {keys[2].ToString("X2")} {keys[3].ToString("X2")}");
        System.Console.WriteLine($"0x25: -- -- -- --");
        System.Console.WriteLine($"0x26: 00 01 00 00 (Token Type)");
        System.Console.WriteLine($"0x2B: {pwd[0].ToString("X2")} {pwd[1].ToString("X2")} {pwd[2].ToString("X2")} {pwd[3].ToString("X2")} (PWD)");
    }

    private static void PrintCharacterResult(byte[] keys, byte[] pwd)
    {
        string name = getCharacterById(_id);

        System.Console.Clear();
        System.Console.WriteLine($"You've picked: {name}");
        System.Console.WriteLine($"0x24: {keys[0].ToString("X2")} {keys[1].ToString("X2")} {keys[2].ToString("X2")} {keys[3].ToString("X2")}");
        System.Console.WriteLine($"0x25: {keys[4].ToString("X2")} {keys[5].ToString("X2")} {keys[6].ToString("X2")} {keys[7].ToString("X2")}");
        System.Console.WriteLine($"0x26: -- -- -- -- (Token Type)");
        System.Console.WriteLine($"0x2B: {pwd[0].ToString("X2")} {pwd[1].ToString("X2")} {pwd[2].ToString("X2")} {pwd[3].ToString("X2")} (PWD)");
    }

    private static string getCharacterById(ushort id)
    {
        string result = Character.Characters.Find(x => x.Id == id).Name;
        if (result == null)
        {
            return "null";
        }
        return result;
    }

    private static string getVehicleById(ushort id)
    {
        string result = Vehicle.Vehicles.Find(x => x.Id == id).Name;
        if (result == null)
        {
            return "null";
        }
        return result;
    }

    private static bool IsHex(IEnumerable<char> chars)
    {
        bool isHex;
        foreach (var c in chars)
        {
            isHex = ((c >= '0' && c <= '9') ||
                     (c >= 'a' && c <= 'f') ||
                     (c >= 'A' && c <= 'F'));

            if (!isHex)
                return false;
        }
        return true;
    }
}