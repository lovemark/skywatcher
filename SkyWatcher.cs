// SkyWatcher core
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SkyWatcher {
    public class SkyObject {
        public string Name;
        public SkyObject() {
            
        }
        public SkyObject(string name) {
            Name = name;
        }
        public override string ToString() {
            return Name;
        }
    }
    [ComVisible(true)]
    public class Star : SkyObject {
        public static int readystars;
        public static int totalstars = 451;
        public double RA;
        public double RA2;
        public double Dec;
        public StarProperties Properties;
        public double Magnitude = 4;
        public bool IsNamed {
            get {
                return !(Name.Contains(" ")) || !(Name.Contains(GetConstellation().Genitive)) || !(Name.Contains("LuisStar-"));
            }
        }
        public Star(string name, double ra, double dec) {
            OnAddedStar(name);
            Name = name;
            RA = ra / 60;
            RA2 = RA - 24;
            Dec = dec;
            CustomColour = Color.FromArgb(70, 255, 255);
        }
        internal static void OnAddedStar(string name) {
            readystars++;
            Console.WriteLine("Initializing {0} ({1}/{2})", name, readystars, totalstars);
        }
        public override string ToString() {
            string nl = "\r\n";
            double ra = Math.Round(RA, 3, MidpointRounding.AwayFromZero);
            return Name + nl + "Coordinates: (" + ra + "h, " + Dec + "\u00b0" + ")" + nl;
        }
        public Point GetLocation(double relative_ra, double relative_dec) {
            double ra = relative_ra - (RA * 60);
            double dec = relative_dec - Dec;
            return new Point((int)(ra * 5), (int)(dec * 20));
        }
        public Point GetLocation2(double relative_ra, double relative_dec) {
            double ra = relative_ra - (RA2 * 60);
            double dec = relative_dec - Dec;
            return new Point((int)(ra * 5), (int)(dec * 20));
        }
        public Constellation GetConstellation() {
            SkyObjectLibrary.Search(Name);
            int index = SkyObjectLibrary.last_index;
            for (int i = index; i >= 0; i--) {
                SkyObject current = SkyObjectLibrary.GetItem(i);
                if (current is Constellation) return (Constellation)(current);
            }
            throw new Exception("Invalid library.");
        }
        static readonly string[] months = {"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"};
        public string GetBestMonth() {
            double temp = RA;
            temp -= 6;
            if (temp < 0) temp += 24;
            temp /= 2;
            int result = (int)(Math.Truncate(temp));
            return months[result];
        }
        public Color CustomColour;
        public Star(string name, double ra, double dec, Color displayColour) : this(name, ra, dec) {
            CustomColour = displayColour;
        }
        public override bool Equals(object obj)
        {
            return Name == ((Star)(obj)).Name;
        }
        public Star(string name, double ra, double dec, StarProperties properties) : this(name, ra, dec) {
            Properties = properties;
        }
        public Star(string name, double ra, double dec, Color displayColour, StarProperties properties) : this(name, ra, dec, displayColour) {
            Properties = properties;
        }
        public Guid GetUUID() {
            SkyObjectLibrary.Search(Name);
            int ra = (int)(Math.Round(RA * 60));
            int dec1 = (int)(Math.Truncate(Dec));
            int dec2 = (int)(Math.Truncate(Dec / 24) - Math.Truncate(Dec / 1440));
            return new Guid(last_index, 0, 24576, 128, 0, (byte)(ra & 255), (byte)(ra / 256), dec1, dec2, 0, 0);
        }
    }
    [Flags]
    public enum StarProperties {
        Normal = 0,
        Double = 1,
        VariableMagnitude = 2
    }
    public sealed class Group : SkyObject {
        public int[] Stars;
        public Group(string name, int[] stars) {
            Star.OnAddedStar(name);
            Name = name;
            Stars = stars;
        }
        public Star[] GetStars() {
            Star[] return_array = new Star[Stars.Length];
            for (int i = 0; i < Stars.Length; i++) {
                return_array[i] = (Star)(SkyObjectLibrary.GetItem(Stars[i]));
            }
            return return_array;
        }
        public Constellation GetConstellation() {
            return ((Star)(SkyObjectLibrary.GetItem(Stars[0]))).GetConstellation();
        }
    }
    public sealed class Constellation : SkyObject {
        public int Start;
        public int End;
        public string Genitive;
        public Constellation(string name, int start, int end, string genitive) {
            Star.OnAddedStar(name);
            Name = name;
            Start = start;
            End = end;
            Genitive = genitive;
        }
        public SkyObject[] GetStars() {
            SkyObject[] return_array = new SkyObject[(End + 1) - Start];
            for (int i = 0; i < (End + 1) - Start; i++) {
                return_array[i] = SkyObjectLibrary.GetItem(i + Start);
            }
            return return_array;
        }
    }
    public class Nebula : Star {
        public Nebula(string name, int ra, int dec) : base(name, ra, dec) {
            CustomColour = Color.FromArgb(243, 255, 128, 255);
        }
    }
    public class Galaxy : Star {
        public Galaxy(string name, int ra, int dec) : base(name, ra, dec) {
            CustomColour = Color.FromArgb(236, Color.Teal);
        }
    }
    public static class SkyObjectLibrary {
        public static SkyObject[] Value;
        public static SkyObject GetItem(int id) {
            return Value[id];
        }
        public static SkyObject Search(string searchText) {
            int result;
            if (int.TryParse(searchText, out result)) {
                if (result < Star.totalstars) {
                    return Value[result];
                }
            }
            for (int i = 0; i < Star.totalstars; i++) {
                if (Value[i] == null) continue;
                if (Value[i].Name == searchText) {
                    last_index = i;
                    return Value[i];
                }
            }
            last_index = -1;
            MessageBox.Show(searchText + " was not found in the program database of " + Star.totalstars + " stars, groups, constellations and more!", "Not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return new SkyObject("Invalid search");
        }
        public static int last_index;
        public static void InitialiseLibrary() {
            // Create the array
            SkyObject[] value = new SkyObject[3000];
            
            // Initialise stars of Andromeda
            value[1] = new Star("Alpheratz", 11, 29, StarProperties.Double);
            value[2] = new Star("Beta Andromedae", 71, 35, StarProperties.Double);
            value[3] = new Star("Gamma Andromedae", 126, 42, StarProperties.Double);
            value[4] = new Star("Delta Andromedae", 43, 31, StarProperties.Double);
            value[5] = new Star("Epsilon Andromedae", 42, 29);
            value[6] = new Star("Zeta Andromedae", 49, 23, StarProperties.VariableMagnitude);
            value[7] = new Star("Eta Andromedae", 57, 22, StarProperties.Double);
            value[8] = new Star("Theta Andromedae", 14, 39);
            value[9] = new Star("Iota Andromedae", 1423, 43, StarProperties.Double);
            value[10] = new Star("Kappa Andromedae", 1425, 44, StarProperties.Double);
            value[11] = new Star("Lambda Andromedae", 1422, 46, StarProperties.VariableMagnitude);
            value[12] = new Star("Mu Andromedae", 58, 39, StarProperties.Double);
            value[13] = new Star("Nu Andromedae", 52, 41);
            value[14] = new Star("Csi Andromedae", 80, 46);
            value[15] = new Star("Omicron Andromedae", 1385, 42, StarProperties.VariableMagnitude);
            value[16] = new Star("Pi Andromedae", 40, 33, StarProperties.Double);
            value[17] = new Star("Rho Andromedae", 22, 38);
            value[18] = new Star("R Andromedae", 24, 39, StarProperties.VariableMagnitude);
            value[19] = new Star("Sigma Andromedae", 23, 37);
            value[20] = new Star("Tau Andromedae", 101, 41, StarProperties.Double);
            value[21] = new Star("Phi Andromedae", 70, 47, StarProperties.Double);
            
            // Initialise stars of Antlia
            value[23] = new Star("Alpha Antliae", 631, -31);
            value[24] = new Star("Delta Antliae", 633, -30, StarProperties.Double);
            value[25] = new Star("Epsilon Antliae", 575, -37);
            value[26] = new Star("Zeta 1 Antliae", 577, -32, StarProperties.Double);
            value[27] = new Star("Zeta 2 Antliae", 581, -32);
            value[28] = new Star("Eta Antliae", 598, -36);
            value[29] = new Star("Theta Antliae", 583, -27, StarProperties.Double);
            value[30] = new Star("Iota Antliae", 655, -37);
            value[31] = new Star("S Antliae", 576, -28, StarProperties.VariableMagnitude);
            value[32] = new Star("U Antliae", 634, -39, StarProperties.VariableMagnitude);
            
            // Initialise stars of Apus
            value[34] = new Star("Alpha Apodis", 886, -79);
            value[35] = new Star("Beta Apodis", 1009, -77);
            value[36] = new Star("Gamma Apodis", 1015, -78);
            value[37] = new Star("Delta 1 Apodis", 977, -79);
            value[38] = new Star("Delta 2 Apodis", 978, -79);
            value[39] = new Star("Epsilon Apodis", 873, -80);
            value[40] = new Star("Zeta Apodis", 1052, -68);
            value[41] = new Star("Theta Apodis", 851, -76, StarProperties.VariableMagnitude);
            value[42] = new Star("Iota Apodis", 1063, -70);
            value[43] = new Star("Kappa 1 Apodis", 932, -73, StarProperties.Double);
            value[44] = new Star("Kappa 2 Apodis", 936, -73, StarProperties.Double);
            
            // Initialise stars of Aquarius
            value[46] = new Star("Alpha Aquarii", 1329, -1);
            value[47] = new Star("Beta Aquarii", 1363, -6, StarProperties.Double);
            value[48] = new Star("Gamma Aquarii", 1343, -2);
            value[49] = new Star("Delta Aquarii", 1374, -16);
            value[50] = new Star("Epsilon Aquarii", 1242, -9);
            value[51] = new Star("Zeta Aquarii", 1352, 0, StarProperties.Double);
            value[52] = new Star("Eta Aquarii", 1355, 0);
            value[53] = new Star("Theta Aquarii", 1337, -8);
            value[54] = new Star("Iota Aquarii", 1323, -14);
            value[55] = new Star("Kappa Aquarii", 1357, -5);
            value[56] = new Star("Lambda Aquarii", 1371, -8);
            value[57] = new Star("Mu Aquarii", 1250, -9);
            value[58] = new Star("Nu Aquarii", 1267, -11);
            value[59] = new Star("Csi Aquarii", 1296, -8);
            value[60] = new Star("Omicron Aquarii", 1322, -2);
            value[61] = new Star("Pi Aquarii", 1345, 1);
            value[62] = new Star("Rho Aquarii", 1341, -8);
            value[63] = new Star("Tau 1 Aquarii", 1361, -4, StarProperties.Double);
            value[64] = new Star("Tau 2 Aquarii", 1364, -3);
            value[65] = new Star("Phi Aquarii", 1397, -6);
            value[66] = new Star("Psi 1 Aquarii", 1396, -9);
            value[67] = new Star("Psi 2 Aquarii", 1398, -9);
            value[68] = new Star("Psi 3 Aquarii", 1399, -9, StarProperties.Double);
            value[69] = new Star("Omega 1 Aquarii", 1421, -4);
            value[70] = new Star("Omega 2 Aquarii", 1424, -5, StarProperties.Double);
            
            // Initialise stars of Aquila
            value[72] = new Star("Altair", 1189, 9);
            value[73] = new Star("Beta Aquilae", 1194, 7, StarProperties.Double);
            value[74] = new Star("Gamma Aquilae", 1182, 11);
            value[75] = new Star("Delta Aquilae", 1164, 3, StarProperties.Double);
            value[76] = new Star("Epsilon Aquilae", 1140, 15);
            value[77] = new Star("Zeta Aquilae", 1145, 3, StarProperties.Double);
            value[78] = new Star("Eta Aquilae", 1186, 1, StarProperties.VariableMagnitude);
            value[79] = new Star("Theta Aquilae", 1213, -1);
            value[80] = new Star("Iota Aquilae", 1178, -1, StarProperties.Double);
            value[81] = new Star("Kappa Aquilae", 1177, -7);
            value[82] = new Star("Lambda Aquilae", 1144, -5);
            value[83] = new Star("Mu Aquilae", 1171, 7, StarProperties.Double);
            value[84] = new Star("Nu Aquilae", 1163, 0);
            value[85] = new Star("Omicron Aquilae", 1187, 11);
            value[86] = new Star("Pi Aquilae", 1188, 12);
            value[87] = new Star("QS Aquilae", 1181, 14, StarProperties.Double | StarProperties.VariableMagnitude);
            value[88] = new Star("Rho Aquilae", 1212, 15);
            value[89] = new Star("R Aquilae", 1147, 8, StarProperties.VariableMagnitude);
            value[90] = new Star("Sigma Aquilae", 1175, 5);
            value[91] = new Star("Tau Aquilae", 1206, 8);
            value[92] = new Star("U Aquilae", 1168, -7, StarProperties.VariableMagnitude);
            value[93] = new Star("Chi Aquilae", 1179, 12);
            value[94] = new Star("Phi Aquilae", 1193, 12);
            value[95] = new Star("Psi Aquilae", 1176, 13);
            
            // Initialise stars of Ara
            value[97] = new Star("Alpha Arae", 1053, -50);
            value[98] = new Star("Beta Arae", 1051, -55);
            value[99] = new Star("Gamma Arae", 1050, -56, StarProperties.Double);
            value[100] = new Star("Delta Arae", 1055, -61);
            value[101] = new Star("Epsilon 1 Arae", 1019, -53);
            value[102] = new Star("Epsilon 2 Arae", 1021, -53);
            value[103] = new Star("Zeta Arae", 1019, -57);
            value[104] = new Star("Eta Arae", 1008, -59);
            value[105] = new Star("Theta Arae", 1087, -50);
            value[106] = new Star("Iota Arae", 1047, -47);
            value[107] = new Star("Kappa Arae", 1044, -51);
            value[108] = new Star("Lambda Arae", 1075, -49);
            value[109] = new Star("Mu Arae", 1063, -52);
            value[110] = new Star("Pi Arae", 1064, -55);
            value[111] = new Star("R Arae", 1001, -57);
            value[112] = new Star("Sigma Arae", 1053, -47);
            value[113] = new Star("U Arae", 1071, -51);
            
            // Initialise stars of Aries
            value[115] = new Star("Alpha Arietis", 128, 23);
            value[116] = new Star("Beta Arietis", 113, 21);
            value[117] = new Star("Gamma Arietis", 111, 19, StarProperties.Double);
            value[118] = new Star("Delta Arietis", 196, 20);
            value[119] = new Star("Epsilon Arietis", 179, 21);
            value[120] = new Star("Zeta Arietis", 197, 21);
            value[121] = new Star("Eta Arietis", 131, 21);
            value[122] = new Star("Theta Arietis", 134, 20);
            value[123] = new Star("Iota Arietis", 119, 18);
            value[124] = new Star("Kappa Arietis", 128, 22);
            value[125] = new Star("Lambda Arietis", 119, 23, StarProperties.Double);
            value[126] = new Star("Mu Arietis", 161, 20);
            value[127] = new Star("Nu Arietis", 157, 22);
            value[128] = new Star("Csi Arietis", 142, 10);
            value[129] = new Star("Omicron Arietis", 165, 15);
            value[130] = new Star("Pi Arietis", 168, 17, StarProperties.Double);
            value[131] = new Star("Rho Arietis", 173, 18);
            value[132] = new Star("Sigma Arietis", 171, 15);
            value[133] = new Star("Tau Arietis", 199, 21);
            value[134] = new Star("U Arietis", 194, 15);
            
            // Initialise stars of Auriga
            value[136] = new Star("Capella", 319, 45);
            value[137] = new Star("Beta Aurigae", 360, 45, StarProperties.VariableMagnitude);
            value[138] = new Star("Delta Aurigae", 360, 54);
            value[139] = new Star("Epsilon Aurigae", 302, 43, StarProperties.VariableMagnitude);
            value[140] = new Star("Zeta Aurigae", 302, 41, StarProperties.VariableMagnitude);
            value[141] = new Star("Eta Aurigae", 304, 41);
            value[142] = new Star("Theta Aurigae", 360, 38, StarProperties.Double);
            value[143] = new Star("Kappa Aurigae", 370, 29);
            value[144] = new Star("Lambda Aurigae", 320, 40, StarProperties.Double);
            value[145] = new Star("Mu Aurigae", 310, 39);
            value[146] = new Star("Nu Aurigae", 354, 39);
            value[147] = new Star("Csi Aurigae", 355, 55);
            value[148] = new Star("Omicron Aurigae", 341, 50);
            value[149] = new Star("Pi Aurigae", 360, 46);
            value[150] = new Star("PU Aurigae", 321, 43, StarProperties.VariableMagnitude);
            value[151] = new Star("Rho Aurigae", 324, 41);
            value[152] = new Star("R Aurigae", 319, 53, StarProperties.VariableMagnitude);
            value[153] = new Star("RT Aurigae", 385, 30, StarProperties.Double | StarProperties.VariableMagnitude);
            value[154] = new Star("Sigma Aurigae", 325, 37);
            value[155] = new Star("Tau Aurigae", 352, 39, StarProperties.Double);
            value[156] = new Star("UU Aurigae", 394, 38, StarProperties.Double | StarProperties.VariableMagnitude);
            
            // Initialise stars of Bootes
            value[158] = new Star("Arcturus", 854, 19, ArcturusColour);
            value[159] = new Star("A Bootis", 852, 35);
            value[160] = new Star("Beta Bootis", 901, 40);
            value[161] = new Star("Gamma Bootis", 869, 38, StarProperties.Double);
            value[162] = new Star("Delta Bootis", 911, 33, StarProperties.Double);
            value[163] = new Star("Epsilon Bootis", 881, 27, StarProperties.Double);
            value[164] = new Star("Zeta Bootis", 883, 13);
            value[165] = new Star("Eta Bootis", 837, 19);
            value[166] = new Star("Theta Bootis", 859, 52);
            value[167] = new Star("Iota Bootis", 851, 51, StarProperties.Double);
            value[168] = new Star("Kappa Bootis", 849, 52, StarProperties.Double);
            value[169] = new Star("Lambda Bootis", 853, 46);
            value[170] = new Star("Mu Bootis", 921, 37, StarProperties.Double);
            value[171] = new Star("Nu 1 Bootis", 932, 41);
            value[172] = new Star("Nu 2 Bootis", 933, 41, StarProperties.Double);
            value[173] = new Star("Csi Bootis", 887, 19, StarProperties.Double);
            value[174] = new Star("Omicron Bootis", 883, 17);
            value[175] = new Star("Pi Bootis", 881, 16, StarProperties.Double);
            value[176] = new Star("Rho Bootis", 870, 30);
            value[177] = new Star("Sigma Bootis", 874, 30);
            value[178] = new Star("Tau Bootis", 826, 17, StarProperties.Double);
            value[179] = new Star("Chi Bootis", 910, 29);
            value[180] = new Star("Phi Bootis", 937, 40);
            value[181] = new Star("Psi Bootis", 904, 27);
            value[182] = new Star("Omega Bootis", 901, 25);
            
            // Initialise stars of Caelum
            value[184] = new Star("Alpha Caeli", 282, -42);
            value[185] = new Star("Beta Caeli", 283, -37);
            value[186] = new Star("Gamma Caeli", 304, -35);
            value[187] = new Star("Delta Caeli", 270, -45);
            value[188] = new Star("Zeta Caeli", 286, -30);
            value[189] = new Star("R Caeli", 282, -38);
            
            // Initialise stars of Camelopardalis
            value[191] = new Star("Alpha Camelopardalis", 293, 66);
            value[192] = new Star("Beta Camelopardalis", 300, 60);
            value[193] = new Star("Gamma Camelopardalis", 231, 71);
            value[194] = new Star("VZ Camelopardalis", 447, 83, StarProperties.Double | StarProperties.VariableMagnitude);
            value[195] = new Star("Z Camelopardalis", 511, 74, StarProperties.VariableMagnitude);
            
            // Initialise stars of Cancer
            value[197] = new Star("Alpha Cancri", 539, 12);
            value[198] = new Star("Beta Cancri", 499, 9);
            value[199] = new Star("Gamma Cancri", 521, 22);
            value[200] = new Star("Delta Cancri", 523, 18, StarProperties.Double);
            value[201] = new Star("Epsilon Cancri", 517, 19);
            value[202] = new Star("Zeta Cancri", 494, 17, StarProperties.Double);
            value[203] = new Star("Eta Cancri", 511, 20);
            value[204] = new Star("Theta Cancri", 509, 18);
            value[205] = new Star("Iota Cancri", 529, 29, StarProperties.Double);
            value[206] = new Star("Kappa Cancri", 549, 11);
            value[207] = new Star("Lambda Cancri", 503, 24);
            value[208] = new Star("Mu Cancri", 488, 22);
            value[209] = new Star("Nu Cancri", 542, 24);
            value[210] = new Star("Csi Cancri", 490, 22);
            value[211] = new Star("Omicron 1 Cancri", 537, 15);
            value[212] = new Star("Omicron 2 Cancri", 538, 16);
            value[213] = new Star("Pi Cancri", 562, 15);
            value[214] = new Star("Rho Cancri", 532, 28, StarProperties.Double);
            value[215] = new Star("R Cancri", 499, 12, StarProperties.VariableMagnitude);
            value[216] = new Star("RS Cancri", 490, 31, StarProperties.Double | StarProperties.VariableMagnitude);
            value[217] = new Star("Sigma Cancri", 536, 33);
            value[218] = new Star("Tau Cancri", 489, 30);
            value[219] = new Star("Chi Cancri", 503, 27);
            value[220] = new Star("Phi 1 Cancri", 508, 28, StarProperties.Double);
            value[221] = new Star("Phi 2 Cancri", 508, 27, StarProperties.Double);
            value[222] = new Star("Psi Cancri", 495, 26);
            value[223] = new Star("Omega Cancri", 480, 26);
            
            // Initialise stars of Canes Venatici
            value[225] = new Star("Cor Caroli", 778, 38, StarProperties.Double);
            value[226] = new Star("Beta Canum Venaticorum", 751, 42);
            value[227] = new Star("R Canum Venaticorum", 821, 40, StarProperties.VariableMagnitude);
            value[228] = new Star("TU Canum Venaticorum", 714, 47, StarProperties.VariableMagnitude);
            value[229] = new Star("Y Canum Venaticorum", 697, 46, StarProperties.VariableMagnitude);
            
            // Initialise stars of Canis Major
            value[231] = new Star("Sirius", 398, -17, StarProperties.Double);
            value[232] = new Star("Beta Canis Majoris", 383, -18);
            value[233] = new Star("Gamma Canis Majoris", 427, -16);
            value[234] = new Star("Wezen", 429, -26);
            value[235] = new Star("Adhara", 419, -29);
            value[236] = new Star("Zeta Canis Majoris", 386, -30);
            value[237] = new Star("Eta Canis Majoris", 447, -29);
            value[238] = new Star("Theta Canis Majoris", 407, -12);
            value[239] = new Star("Iota Canis Majoris", 413, 13);
            value[240] = new Star("Kappa Canis Majoris", 405, -33);
            value[241] = new Star("Lambda Canis Majoris", 387, -33);
            value[242] = new Star("Mu Canis Majoris", 413, -14, StarProperties.Double);
            value[243] = new Star("Nu Canis Majoris", 411, -19, StarProperties.Double);
            value[244] = new Star("Csi Canis Majoris", 409, -24, StarProperties.Double);
            value[245] = new Star("Omicron 1 Canis Majoris", 413, -24);
            value[246] = new Star("Omicron 2 Canis Majoris", 423, -24);
            value[247] = new Star("Pi Canis Majoris", 413, -20, StarProperties.Double);
            value[248] = new Star("R Canis Majoris", 380, -16, StarProperties.VariableMagnitude);
            value[249] = new Star("Sigma Canis Majoris", 362, -28, StarProperties.Double);
            value[250] = new Star("Tau Canis Majoris", 441, -25, StarProperties.Double);
            value[251] = new Star("Omega Canis Majoris", 399, -27);
            
            // Initialise stars of Canis Minor
            value[253] = new Star("Procyon", 456, 5, ProcyonColour, StarProperties.Double);
            value[254] = new Star("Beta Canis Minoris", 445, 8);
            value[255] = new Star("Gamma Canis Minoris", 446, 9, StarProperties.Double);
            value[256] = new Star("Delta 1 Canis Minoris", 447, 2);
            value[257] = new Star("Delta 2 Canis Minoris", 450, 4);
            value[258] = new Star("Delta 3 Canis Minoris", 451, 4);
            value[259] = new Star("Epsilon Canis Minoris", 444, 9);
            value[260] = new Star("Zeta Canis Minoris", 467, 2);
            
            // Initialise stars of Capricornus
            value[262] = new Star("Alpha 1 Capricorni", 1217, -13, StarProperties.Double);
            value[263] = new Star("Alpha 2 Capricorni", 1219, -13, StarProperties.Double);
            value[264] = new Star("Beta Capricorni", 1224, -15);
            value[265] = new Star("Gamma Capricorni", 1301, -16);
            value[266] = new Star("Delta Capricorni", 1306, -14);
            value[267] = new Star("Epsilon Capricorni", 1299, -20, StarProperties.Double | StarProperties.VariableMagnitude);
            value[268] = new Star("Zeta Capricorni", 1289, -23);
            value[269] = new Star("Eta Capricorni", 1267, -20);
            value[270] = new Star("Theta Capricorni", 1269, -17);
            value[271] = new Star("Iota Capricorni", 1284, -17);
            value[272] = new Star("Kappa Capricorni", 1302, -19);
            value[273] = new Star("Lambda Capricorni", 1301, -18);
            value[274] = new Star("Mu Capricorni", 1312, -14);
            value[275] = new Star("Nu Capricorni", 1220, -13, StarProperties.Double);
            value[276] = new Star("Csi Capricorni", 1212, -13, StarProperties.Double);
            value[277] = new Star("Omicron Capricorni", -18, StarProperties.Double);
            value[278] = new Star("Pi Capricorni", 1245, -18, StarProperties.Double);
            value[279] = new Star("Rho Capricorni", 1246, -18, StarProperties.Double);
            value[280] = new Star("RT Capricorni", 1219, -22, StarProperties.VariableMagnitude);
            value[281] = new Star("Sigma Capricorni", 1219, -18, StarProperties.Double);
            value[282] = new Star("Tau Capricorni", 1235, -15, StarProperties.Double);
            value[283] = new Star("Chi Capricorni", 1272, -21, StarProperties.Double);
            value[284] = new Star("Phi Capricorni", 1276, -20);
            value[285] = new Star("Psi Capricorni", 1241, -27);
            value[286] = new Star("Omega Capricorni", 1245, -28);
            
            // Initialise stars of Carina
            value[288] = new Star("Canopus", 392, -53);
            value[289] = new Star("A Carinae", 410, -54);
            value[290] = new Star("LuisStar-0290", 553, -59, StarProperties.Double);
            value[291] = new Star("Beta Carinae", 559, -70);
            value[292] = new Star("LuisStar-0292", 538, -61);
            value[293] = new Star("LuisStar-0293", 529, -60, StarProperties.VariableMagnitude);
            value[294] = new Star("Epsilon Carinae", 511, -59);
            value[295] = new Star("LuisStar-0295", 529, -56);
            value[296] = new Star("Eta Carinae", 650, -60, StarProperties.VariableMagnitude);
            value[297] = new Star("LuisStar-0297", 560, -57);
            value[298] = new Star("LuisStar-0298", 566, -59);
            value[299] = new Star("Theta Carinae", 652, -64);
            value[300] = new Star("G Carinae", 550, -73);
            value[301] = new Star("Chi Carinae", 473, -53);
            value[302] = new Star("Omega Carinae", 621, -70);
            
            // Initialise stars of Cassiopeia
            value[304] = new Star("Alpha Cassiopeiae", 41, 56, StarProperties.Double);
            value[305] = new Star("Beta Cassiopeiae", 14, 59, StarProperties.Double);
            value[306] = new Star("Gamma Cassiopeiae", 60, 61, StarProperties.Double | StarProperties.VariableMagnitude);
            value[307] = new Star("Delta Cassiopeiae", 83, 60, StarProperties.VariableMagnitude);
            value[308] = new Star("Epsilon Cassiopeiae", 109, 64);
            value[309] = new Star("Zeta Cassiopeiae", 41, 54);
            value[310] = new Star("Eta Cassiopeiae", 51, 57, StarProperties.Double);
            value[311] = new Star("Theta Cassiopeiae", 80, 55);
            value[312] = new Star("Iota Cassiopeiae", 155, 68, StarProperties.Double);
            value[313] = new Star("Kappa Cassiopeiae", 39, 63);
            value[314] = new Star("Lambda Cassiopeiae", 36, 54, StarProperties.Double);
            value[315] = new Star("Mu Cassiopeiae", 73, 55);
            value[316] = new Star("Nu Cassiopeiae", 52, 51);
            value[317] = new Star("Csi Cassiopeiae", 49, 50);
            value[318] = new Star("Omicron Cassiopeiae", 48, 48, StarProperties.Double);
            value[319] = new Star("Pi Cassiopeiae", 48, 47);
            value[320] = new Star("Rho Cassiopeiae", 1434, 57, StarProperties.VariableMagnitude);
            value[321] = new Star("R Cassiopeiae", 1437, 51, StarProperties.VariableMagnitude);
            value[322] = new Star("RZ Cassiopeiae", 168, 70, StarProperties.VariableMagnitude);
            value[323] = new Star("Sigma Cassiopeiae", 0, 56);
            value[324] = new Star("Tau Cassiopeiae", 1416, 58);
            value[325] = new Star("Chi Cassiopeiae", 90, 59);
            value[326] = new Star("Phi Cassiopeiae", 84, 58, StarProperties.Double);
            value[327] = new Star("Psi Cassiopeiae", 79, 68, StarProperties.Double);
            value[328] = new Star("Omega Cassiopeiae", 117, 59);
            
            // Initialise stars of Centaurus
            value[330] = new Star("Rigel Kentaurus", 881, -60, StarProperties.Double);
            value[331] = new Star("Hadar", 841, -61, StarProperties.Double);
            value[332] = new Star("Gamma Centauri", 767, -49, StarProperties.Double);
            value[333] = new Star("Delta Centauri", 728, -51);
            value[334] = new Star("Epsilon Centauri", 816, -54, StarProperties.Double);
            value[335] = new Star("Zeta Centauri", 832, -47);
            value[336] = new Star("Eta Centauri", 851, -43, StarProperties.Double);
            value[337] = new Star("Theta Centauri", 848, -36);
            value[338] = new Star("Iota Centauri", 805, -37, StarProperties.Double);
            value[339] = new Star("Kappa Centauri", 900, -42, StarProperties.Double);
            value[340] = new Star("Lambda Centauri", 695, -64);
            value[341] = new Star("Mu Centauri", 823, -42, StarProperties.VariableMagnitude);
            value[342] = new Star("Nu Centauri", 823, -41);
            value[343] = new Star("Csi Centauri", 784, -50, StarProperties.Double);
            value[344] = new Star("Omicron Centauri", 690, -60, StarProperties.Double);
            value[345] = new Star("Pi Centauri", 687, -54, StarProperties.Double);
            value[346] = new Star("Rho Centauri", 735, -52);
            value[347] = new Star("R Centauri", 859, -60, StarProperties.VariableMagnitude);
            value[348] = new Star("Sigma Centauri", 749, -50);
            value[349] = new Star("S Centauri", 747, -49, StarProperties.VariableMagnitude);
            value[350] = new Star("Tau Centauri", 733, -48);
            value[351] = new Star("T Centauri", 814, -34, StarProperties.VariableMagnitude);
            value[352] = new Star("V Centauri", 872, -57, StarProperties.VariableMagnitude);
            value[353] = new Star("Psi Centauri", 861, -38);
            
            // Initialise stars of Cepheus
            value[355] = new Star("Alpha Cephei", 1246, 63);
            value[356] = new Star("Beta Cephei", 1281, 71, StarProperties.Double | StarProperties.VariableMagnitude);
            value[357] = new Star("Gamma Cephei", 1419, 77);
            value[358] = new Star("Delta Cephei", 1359, 58, StarProperties.VariableMagnitude);
            value[359] = new Star("Epsilon Cephei", 1341, 57);
            value[360] = new Star("Zeta Cephei", 1328, 58);
            value[361] = new Star("Eta Cephei", 1250, 62, StarProperties.Double);
            value[362] = new Star("Theta Cephei", 1238, 63);
            value[363] = new Star("Iota Cephei", 1375, 66);
            value[364] = new Star("Kappa Cephei", 1209, 77, StarProperties.Double);
            value[365] = new Star("Lambda Cephei", 1326, 60);
            value[366] = new Star("Mu Cephei", 1295, 58, StarProperties.VariableMagnitude);
            value[367] = new Star("Nu Cephei", 1298, 62);
            value[368] = new Star("Csi Cephei", 1321, 65, StarProperties.Double);
            value[369] = new Star("Omicron Cephei", 1389, 68, StarProperties.Double);
            value[370] = new Star("Pi Cephei", 1379, 76, StarProperties.Double);
            value[371] = new Star("Rho Cephei", 1361, 78);
            value[372] = new Star("RW Cephei", 1351, 56, StarProperties.VariableMagnitude);
            value[373] = new Star("T Cephei", 1273, 68, StarProperties.VariableMagnitude);
            value[374] = new Star("VV Cephei", 1314, 64, StarProperties.VariableMagnitude);
            
            // Initialise constellations
            value[0] = new Constellation("Andromeda", 1, 21, "Andromedae");
            value[22] = new Constellation("Antlia", 23, 32, "Antliae");
            value[33] = new Constellation("Apus", 34, 44, "Apodis");
            value[45] = new Constellation("Aquarius", 46, 70, "Aquarii");
            value[71] = new Constellation("Aquila", 72, 95, "Aquilae");
            value[96] = new Constellation("Ara", 97, 113, "Arae");
            value[114] = new Constellation("Aries", 115, 134, "Arietis");
            value[135] = new Constellation("Auriga", 136, 156, "Aurigae");
            value[157] = new Constellation("Bootes", 158, 182, "Bootis");
            value[183] = new Constellation("Caelum", 184, 189, "Caeli");
            value[190] = new Constellation("Camelopardalis", 191, 195, "Camelopardalis");
            value[197] = new Constellation("Cancer", 196, 223, "Cancri");
            value[224] = new Constellation("Canes Venatici", 225, 229, "Canum Venaticorum");
            value[230] = new Constellation("Canis Major", 231, 251, "Canis Majoris");
            value[252] = new Constellation("Canis Minor", 253, 260, "Canis Minoris");
            value[261] = new Constellation("Capricornus", 262, 286, "Capricorni");
            value[287] = new Constellation("Carina", 288, 302, "Carinae");
            value[303] = new Constellation("Cassiopeia", 304, 328, "Cassiopeiae");
            value[329] = new Constellation("Centaurus", 330, 353, "Centauri");
            value[354] = new Constellation("Cepheus", 355, 374, "Cephei");
            
            // If you have new SkyObjects, insert them in InitialiseLibrary6 or in the following methods.
            // Examples:
            // M 13: insert it between the comment 'Hercules' and the next empty line
            // I already don't know the value of the ra and dec variables in the code below.
            // value[2013] = new GroupAsStar("M 13", ra, dec);
            // Psi 1 to 9 Aurigae: I've missed these stars.
            // Insert in InitialiseLibrary7 (example and with ra and dec values unknown):
            // value[2120] = new Star("Psi 1 Aurigae", ra, dec);
            // This project is available on SourceForge at:
            // http://sourceforge.net/projects/skywatcher
            // I have an issue (#3). Do a pull request in https://github.com/luismark/skywatcher
            // before the 12nd of October.
            
            // Calls to the following methods
            InitialiseLibrary2(value);
            InitialiseLibrary3(value);
            InitialiseLibrary4(value);
            InitialiseLibrary5(value);
            InitialiseLibrary6(value);
            
            Value = value;
        }
        public static void InitialiseLibrary2(SkyObject[] value) {
            // Initialise stars of Cetus
            value[376] = new Star("Alpha Ceti", 183, 4);
            value[377] = new Star("Beta Ceti", 42, -18);
            value[378] = new Star("Gamma Ceti", 161, 3, StarProperties.Double);
            value[379] = new Star("Delta Ceti", 156, 0);
            value[380] = new Star("Epsilon Ceti", 159, -12);
            value[381] = new Star("Zeta Ceti", 112, -10);
            value[382] = new Star("Eta Ceti", 71, -10);
            value[383] = new Star("Theta Ceti", 85, -8);
            value[384] = new Star("Iota Ceti", 20, -9);
            value[385] = new Star("Kappa Ceti", 201, 3);
            value[386] = new Star("Lambda Ceti", 180, 9);
            value[387] = new Star("Mu Ceti", 163, 10);
            value[388] = new Star("Nu Ceti", 153, 6, StarProperties.Double);
            value[389] = new Star("Csi Ceti", 145, 8, StarProperties.Double);
            value[389] = new Star("Mira", 141, -3, StarProperties.VariableMagnitude);
            value[390] = new Star("Pi Ceti", 161, 6);
            value[391] = new Star("Rho Ceti", 143, -12);
            value[392] = new Star("Sigma Ceti", 153, -15);
            value[393] = new Star("Tau Ceti", 101, -17);
            
            // Initialise stars of Chamaeleon
            value[395] = new Star("Alpha Chamaeleontis", 509, -77);
            value[396] = new Star("Beta Chamaeleontis", 751, -79);
            value[397] = new Star("Gamma Chamaeleontis", 640, -78);
            value[398] = new Star("Delta Chamaeleontis", 651, -81, StarProperties.Double);
            value[399] = new Star("Epsilon Chamaeleontis", 720, -78, StarProperties.Double);
            value[400] = new Star("Zeta Chamaeleontis", 569, -81);
            value[401] = new Star("Eta Chamaeleontis", 522, -79);
            value[402] = new Star("Theta Chamaeleontis", 509, -78, StarProperties.Double);
            value[403] = new Star("RS Chamaeleontis", 528, -79, StarProperties.VariableMagnitude);
            value[404] = new Star("Z Chamaeleontis", 490, -77, StarProperties.VariableMagnitude);
            
            // Initialise stars of Circinus
            value[406] = new Star("Alpha Circini", 891, -66, StarProperties.Double);
            value[407] = new Star("Beta Circini", 909, -59);
            value[408] = new Star("Gamma Circini", 913, -59, StarProperties.Double);
            value[409] = new Star("Delta Circini", 913, -62);
            value[410] = new Star("Epsilon Circini", 906, -64);
            value[411] = new Star("Zeta Circini", 896, -66);
            value[412] = new Star("Eta Circini", 899, -64);
            value[413] = new Star("Theta Circini", 894, -63, StarProperties.Double);
            
            // Initialise stars of Columba
            value[415] = new Star("Alpha Columbae", 326, -34, StarProperties.Double);
            value[416] = new Star("Beta Columbae", 344, -36);
            value[417] = new Star("Gamma Columbae", 358, -36);
            value[418] = new Star("Delta Columbae", 385, -34);
            value[419] = new Star("Epsilon Columbae", 327, -36);
            value[420] = new Star("Eta Columbae", 358, -43);
            value[421] = new Star("Theta Columbae", 371, -37);
            value[422] = new Star("Kappa Columbae", 382, -36);
            value[423] = new Star("SX Columbae", 392, -36, StarProperties.Double);
            value[424] = new Star("T Columbae", 319, -34, StarProperties.VariableMagnitude);
            
            // Initialise stars of Coma Berenices
            value[426] = new Star("Alpha Comae Berenices", 795, 17, StarProperties.Double);
            value[427] = new Star("Beta Comae Berenices", 796, 27);
            value[428] = new Star("Gamma Comae Berenices", 748, 28);
            
            // Initialise stars of Corona Australis
            value[430] = new Star("Alpha Coronae Australis", 1158, -37);
            value[431] = new Star("Beta Coronae Australis", 1158, -39);
            value[432] = new Star("Gamma Coronae Australis", 1155, -37);
            value[433] = new Star("Delta Coronae Australis", 1154, -41);
            value[434] = new Star("Epsilon Coronae Australis", 1139, -37, StarProperties.VariableMagnitude);
            value[435] = new Star("Zeta Coronae Australis", 1141, -43);
            
            // Initialise stars of Corona Borealis
            value[437] = new Star("Alpha Coronae Borealis", 933, 26, StarProperties.VariableMagnitude);
            value[438] = new Star("Beta Coronae Borealis", 928, 29);
            value[439] = new Star("Gamma Coronae Borealis", 940, 26, StarProperties.Double);
            value[440] = new Star("Delta Coronae Borealis", 944, 26);
            value[441] = new Star("Epsilon Coronae Borealis", 958, 26, StarProperties.Double);
            value[442] = new Star("Theta Coronae Borealis", 931, 32);
            value[443] = new Star("R Coronae Borealis", 940, 27, StarProperties.VariableMagnitude);
            
            // Initialise stars of Corvus
            value[445] = new Star("Alpha Corvi", 735, -25);
            value[446] = new Star("Beta Corvi", 754, -24);
            value[447] = new Star("Gamma Corvi", 740, -17);
            value[448] = new Star("Delta Corvi", 752, -16, StarProperties.Double);
            value[449] = new Star("Epsilon Corvi", 735, -23);
            value[450] = new Star("R Corvi", 741, -19, StarProperties.VariableMagnitude);
            
            // Initialise stars of Crater
            value[452] = new Star("Alpha Crateris", 660, -17);
            value[453] = new Star("Beta Crateris", 676, -23);
            value[454] = new Star("Gamma Crateris", 688, -18, StarProperties.Double);
            value[455] = new Star("Delta Crateris", 680, -15);
            value[456] = new Star("Epsilon Crateris", 688, -11);
            value[457] = new Star("Zeta Crateris", 702, -18);
            value[458] = new Star("Eta Crateris", 713, -17);
            value[459] = new Star("Theta Crateris", 695, -10);
            value[460] = new Star("Iota Crateris", 696, -14, StarProperties.Double);
            value[461] = new Star("Kappa Crateris", 690, -13);
            
            // Initialise stars of Crux
            value[463] = new Star("Acrux", 755, -64, StarProperties.Double);
            value[464] = new Star("Mimosa", 771, -60, StarProperties.Double);
            value[465] = new Star("Gacrux", 757, -56, StarProperties.Double);
            value[466] = new Star("Delta Crucis", 731, -59);
            value[467] = new Star("Epsilon Crucis", 747, -60);
            value[468] = new Star("Zeta Crucis", 749, -65);
            value[469] = new Star("Eta Crucis", 731, -65, StarProperties.Double);
            value[470] = new Star("Theta Crucis", 723, -64, StarProperties.Double);
            
            // Initialise stars of Cygnus
            value[472] = new Star("Deneb", 1240, 45);
            value[473] = new Star("Albireo", 1168, 27, AlbireoColour, StarProperties.Double);
            value[474] = new Star("Sadr", 1226, 40, StarProperties.Double);
            value[475] = new Star("Delta Cygni", 1182, 45, StarProperties.Double);
            value[476] = new Star("Epsilon Cygni", 1241, 33, StarProperties.Double);
            value[477] = new Group("Northern Cross", new int[5]{472, 473, 474, 475, 476});
            value[478] = new Star("Zeta Cygni", 1278, 30);
            value[479] = new Star("Epsilon Cygni", 1198, 35, StarProperties.Double);
            value[480] = new Star("Theta Cygni", 1177, 50);
            value[481] = new Star("Iota Cygni", 1168, 51);
            value[482] = new Star("Kappa Cygni", 1160, 54);
            value[483] = new Star("Lambda Cygni", 1161, 36, StarProperties.Double);
            value[484] = new Star("Mu Cygni", 1302, 29, StarProperties.Double);
            value[485] = new Star("Nu Cygni", 1258, 41);
            value[486] = new Star("Csi Cygni", 1267, 44);
            value[487] = new Star("Omicron 1 Cygni", 1220, 46);
            value[488] = new Star("Omicron 2 Cygni", 1221, 48);
            value[489] = new Star("Pi 1 Cygni", 1295, 51);
            value[490] = new Star("Pi 2 Cygni", 1305, 49);
            value[491] = new Star("Rho Cygni", 1292, 45);
            value[492] = new Star("R Cygni", 1174, 50, StarProperties.VariableMagnitude);
            value[493] = new Star("RT Cygni", 1181, 49, StarProperties.VariableMagnitude);
            value[494] = new Star("Sigma Cygni", 1280, 39);
            value[495] = new Star("Tau Cygni", 1278, 38, StarProperties.Double);
            value[496] = new Star("Chi Cygni", 1189, 33, StarProperties.VariableMagnitude);
            value[497] = new Star("Phi Cygni", 1179, 30);
            value[498] = new Star("Psi Cygni", 1189, 53, StarProeprties.Double);
            value[499] = new Star("Omega 1 Cygni", 1232, 48);
            value[500] = new Star("Omega 2 Cygni", 1230, 49);
            
            // Initialise stars of Delphinus
            value[502] = new Star("Alpha Delphini", 1240, 16, StarProperties.Double);
            value[503] = new Star("Beta Delphini", 1239, 15, StarProperties.Double);
            value[504] = new Star("Gamma Delphini", 1245, 16, StarProperties.Double);
            value[505] = new Star("Delta Delphini", 1241, 15);
            value[506] = new Star("Epsilon Delphini", 1232, 15);
            value[507] = new Star("R Delphini", 1217, 9, StarProperties.VariableMagnitude);
            
            // Initialise constellations (second time)
            value[375] = new Constellation("Cetus", 376, 393, "Ceti");
            value[394] = new Constellation("Chamaeleon", 395, 404, "Chamaeleontis");
            value[405] = new Constellation("Circinus", 406, 413, "Circini");
            value[414] = new Constellation("Columba", 415, 424, "Columbae");
            value[425] = new Constellation("Coma Berenices", 426, 428, "Comae Berenices");
            value[429] = new Constellation("Corona Australis", 430, 435, "Coronae Australis");
            value[436] = new Constellation("Corona Borealis", 437, 443, "Coronae Borealis");
            value[444] = new Constellation("Corvus", 445, 450, "Corvi");
            value[451] = new Constellation("Crater", 452, 461, "Crater");
            value[462] = new Constellation("Crux", 463, 470, "Crucis");
            value[471] = new Constellation("Cygnus", 472, 500, "Cygni");
            value[501] = new Constellation("Delphinus", 502, 507, "Delphini");
        }
        public static void InitialiseLibrary3(SkyObject[] value) {
            // This method is for extra constellations.
            // For technical reasons, a method only supports 20 constellations at once.
        }
        public static void InitialiseLibrary4(SkyObject[] value) {
            // This method is for extra constellations.
            // For technical reasons, a method only supports 20 constellations at once.
        }
        public static void InitialiseLibrary5(SkyObject[] value) {
            // This method is for extra constellations.
            // For technical reasons, a method only supports 20 constellations at once.
        }
        public static void InitialiseLibrary6(SkyObject[] value) {
            // This method is for initialising Messier objects and magnitudes.
            // For technical reasons, a method only supports 20 constellations at once.
            value[231].Magnitude = -1.46;
            value[288].Magnitude = -0.72;
            value[331].Magnitude = -0.01;
            value[158].Magnitude = -0.04;
            value[136].Magnitude = 0.08;
            value[253].Magnitude = 0.8;
            value[332].Magnitude = 0.66;
            value[72].Magnitude = 0.77;
            value[463].Magnitude = 0.87;
            value[472].Magnitude = 1.25;
            value[464].Magnitude = 1.28;
            value[235].Magnitude = 1.50;
            value[234].Magnitude = 2.12;
        }
        public static Constellation Andromeda {
            get {
                return (Constellation)(Value[AndromedaLocation]);
            }
        }
        public static int AndromedaLocation = 0;
        public static Constellation Antlia {
            get {
                return (Constellation)(Value[AntliaLocation]);
            }
        }
        public static int AntliaLocation = 22;
        public static Constellation Apus {
            get {
                return (Constellation)(Value[ApusLocation]);
            }
        }
        public static int ApusLocation = 33;
        public static Constellation Aquarius {
            get {
                return (Constellation)(Value[AquariusLocation]);
            }
        }
        public static int AquariusLocation = 45;
        public static Constellation Aquila {
            get {
                return (Constellation)(Value[AquilaLocation]);
            }
        }
        public static int AquilaLocation = 71;
        public static Constellation Ara {
            get {
                return (Constellation)(Value[AraLocation]);
            }
        }
        public static int AraLocation = 96;
        public static Constellation Aries {
            get {
                return (Constellation)(Value[AriesLocation]);
            }
        }
        public static int AriesLocation = 114;
        public static Constellation Auriga {
            get {
                return (Constellation)(Value[AurigaLocation]);
            }
        }
        public static int AurigaLocation = 135;
        public static Constellation Bootes {
            get {
                return (Constellation)(Value[BootesLocation]);
            }
        }
        public static int BootesLocation = 157;
        public static Constellation Caelum {
            get {
                return (Constellation)(Value[CaelumLocation]);
            }
        }
        public static int CaelumLocation = 183;
        public static Constellation Camelopardalis {
            get {
                return (Constellation)(Value[CamelopardalisLocation]);
            }
        }
        public static int CamelopardalisLocation = 190;
        public static Constellation Cancer {
            get {
                return (Constellation)(Value[CancerLocation]);
            }
        }
        public static int CancerLocation = 196;
        public static Constellation CanesVenatici {
            get {
                return (Constellation)(Value[CanesVenaticiLocation]);
            }
        }
        public static int CanesVenaticiLocation = 224;
        public static Constellation CanisMajor {
            get {
                return (Constellation)(Value[CanisMajorLocation]);
            }
        }
        public static int CanisMajorLocation = 230;
        public static Constellation CanisMinor {
            get {
                return (Constellation)(Value[CanisMinorLocation]);
            }
        }
        public static int CanisMinorLocation = 252;
        public static Constellation Capricornus {
            get {
                return (Constellation)(Value[CapricornusLocation]);
            }
        }
        public static int CapricornusLocation = 261;
        public static Constellation Carina {
            get {
                return (Constellation)(Value[CarinaLocation]);
            }
        }
        public static int CarinaLocation = 287;
        public static Constellation Cassiopeia {
            get {
                return (Constellation)(Value[CassiopeiaLocation]);
            }
        }
        public static int CassiopeiaLocation = 303;
        public static Constellation Centaurus {
            get {
                return (Constellation)(Value[CentaurusLocation]);
            }
        }
        public static int CentaurusLocation = 329;
        public static Color ArcturusColour = Color.FromArgb(255, 226, 0);
        public static Color ProcyonColour = Color.FromArgb(255, 226, 0);
        public static Color AlbireoColour = Color.FromArgb(255, 226, 0);
    }
}

