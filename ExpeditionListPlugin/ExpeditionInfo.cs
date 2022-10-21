using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ExpeditionListPlugin
{
    public class ExpeditionInfo
    {
        /// <summary>
        /// 駆逐艦
        /// </summary>
        public static readonly string DESTROYER = "(?<駆>駆逐艦)";

        /// <summary>
        /// 軽巡洋艦
        /// </summary>
        public static readonly string LIGHTCRUISER = "(?<軽>軽巡洋艦)";

        /// <summary>
        /// 重巡洋艦
        /// </summary>
        public static readonly string HEAVYCRUISER = "(?<重>重巡洋艦)";

        /// <summary>
        /// 航空戦艦
        /// </summary>
        public static readonly string AVIATIONBATTLESHIP = "(?<航戦>航空戦艦)";

        /// <summary>
        /// 空母
        /// </summary>
        public static readonly string AIRCRAFTCARRIER = "(?<空母>軽空母|正規空母|装甲空母|水上機母艦|護衛空母)";

        /// <summary>
        /// 水上機母艦
        /// </summary>
        public static readonly string SEAPLANETENDER = "(?<水母>水上機母艦)";

        /// <summary>
        /// 護衛空母
        /// </summary>
        public static readonly string ESCORTECARRIER = "(?<護母>護衛空母)";

        /// <summary>
        /// 海防艦
        /// </summary>
        public static readonly string ESCORT = "(?<海防>海防艦)";

        /// <summary>
        /// 練習巡洋艦
        /// </summary>
        public static readonly string TRAININGCRUISER = "(?<練>練習巡洋艦)";

        /// <summary>
        /// 潜水艦
        /// </summary>
        public static readonly string SUBMARINE = "(?<潜>潜水艦|潜水空母)";

        /// <summary>
        /// 潜水母艦
        /// </summary>
        public static readonly string SUBMARINETENDER = "(?<潜母艦>潜水母艦)";

        /// <summary>
        /// ドラム缶
        /// </summary>
        public static readonly string DRUMCANISTER = "ドラム缶(輸送用)";

        public string Area { get; set; }
        public string ID { get; set; }
        public string EName { get; set; }
        public string Time { get; set; }
        public bool isSuccess2 { get; set; } = false;
        public bool isSuccess3 { get; set; } = false;
        public bool isSuccess4 { get; set; } = false;
        public int Lv { get; set; }
        public int? SumLv { get; set; }
        public int ShipNum { get; set; }

        /// <summary>
        /// 必要艦種
        /// </summary>
        /// <value>
        /// 必要艦種名と数
        /// </value>
        public Dictionary<Tuple<string, int>[], Tuple<string, int>[]> RequireShipType { get; set; }
        public Dictionary<string, int> RequireItemNum { get; set; }
        public Dictionary<string, int> RequireItemShipNum { get; set; }
        public string FlagShipType { get; set; }
        public string FlagShipTypeText
        {
            get
            {
                if (null == FlagShipType) return "";

                Regex regexp = new Regex("<(.+)>(.+)");
                var match = regexp.Match(FlagShipType);

                return match.Success ? match.Groups[1].ToString() : "";
            }
        }

        /// <summary>
        /// 必要艦種テキスト
        /// </summary>
        public string RequireShipTypeText => string.Join("/", new[] { RequireShipTypeTextInner, RequireSumShipTypeText }.Where(x => x != string.Empty));

        public string RequireShipTypeTextInner => RequireShipType == null ? string.Empty :
            string.Join(" ",
                RequireShipType.Select(x =>
                    string.Join("", x.Key.Select(y => $"{Regex.Replace(y.Item1, @".+<(.+)>.+", "$1")}{y.Item2}"))
                    + (x.Value == null ? string.Empty : string.Join("or", x.Value.Select(z => $"{Regex.Replace(z.Item1, @".+<(.+)>.+", "$1")}{z.Item2}")))));

        public string RequireDrum => (null == RequireItemNum || null == RequireItemShipNum) ? string.Empty : $"{RequireItemShipNum[DRUMCANISTER]}隻 {RequireItemNum[DRUMCANISTER]}個";

        /// <summary>
        /// 必要合算艦種
        /// </summary>
        public string[] RequireSumShipType { get; set; }

        /// <summary>
        /// 必要合算艦種数
        /// </summary>
        public int RequireSumShipTypeNum { get; set; }

        /// <summary>
        /// 必要合算艦種テキスト
        /// </summary>
        public string RequireSumShipTypeText => null == RequireSumShipType ? string.Empty : $"{string.Join(",", RequireSumShipType.Select(x => $"{Regex.Replace(x, @".+<(.+)>.+", "$1")}"))}合計{RequireSumShipTypeNum}";

        /// <summary>
        /// 合計対空値
        /// </summary>
        public int? SumAA { get; set; }

        /// <summary>
        /// 合計対潜値
        /// </summary>
        public int? SumASW { get; set; }

        /// <summary>
        /// 合計索敵値
        /// </summary>
        public int? SumViewRange { get; set; }

        public static readonly string AA = "対空";
        public static readonly string ASW = "対潜";
        public static readonly string VIEWRANGE = "索敵";

        /// <summary>
        /// 第二～四艦隊パラメータ
        /// </summary>
        /// <value>
        /// 対空、対潜、索敵が要件を満たしているか。条件がない場合はnull
        /// </value>
        public Dictionary<int, Dictionary<string, bool?>> isParameter { get; set; } = new Dictionary<int, Dictionary<string, bool?>>();

        /// <summary>
        /// 第二～四艦隊パラメータの要件を満たしているか
        /// </summary>
        /// <value>
        /// 対空、対潜、索敵がひとつでもfalseの場合はfalseそうでない場合はtrue
        /// </value>
        public Dictionary<int,bool> isParameterValid
        {
            get
            {
                for (var i = 2; i <= 4; i++)
                {
                    _isParameterValid[i] = isParameter[i].Any(p => p.Value == false) == true ? false : true;
                }

                return _isParameterValid;
            }
        }

        private Dictionary<int, bool> _isParameterValid = new Dictionary<int, bool>();

        /// <summary>
        /// 必須合計パラメータ
        /// </summary>
        public string RequireSumParamText
        {
            get
            {
                var buf = new string[] {
                    SumAA != null ? AA + SumAA.ToString() : string.Empty,
                    SumASW != null ? ASW + SumASW.ToString() : string.Empty,
                    SumViewRange != null ? VIEWRANGE + SumViewRange.ToString() : string.Empty};

                return string.Join("/", buf.Where(s => s.Length > 0));
            }
        }

        public int? Exp { get; set; }
        public int? Fuel { get; set; }
        public int? Ammunition { get; set; }
        public int? Bauxite { get; set; }
        public int? Steel { get; set; }
        public int? InstantRepairMaterials { get; set; }
        public int? InstantBuildMaterials { get; set; }
        public int? DevelopmentMaterials { get; set; }
        public int? AlterationMaterials { get; set; }

        /// <summary>
        /// 家具箱
        /// </summary>
        public string FurnitureBox { get; set; }

        public bool isFuelNull { get { return Fuel == null; } }
        public bool isAmmunitionNull { get { return Ammunition == null; } }
        public bool isSteelNull { get { return Steel == null; } }
        public bool isBauxiteNull { get { return Bauxite == null; } }

        public static ExpeditionInfo[] _ExpeditionTable = new ExpeditionInfo[]
        {
            new ExpeditionInfo {Area="鎮守府", ID="1", EName="練習航海", Time="00:15:00", Lv=1, ShipNum=2, Exp=10, Ammunition=30},
            new ExpeditionInfo {Area="鎮守府", ID="2", EName="長距離練習航海", Time="00:30:00", Lv=2, ShipNum=4, Exp=20, Ammunition=100, Steel=30, InstantRepairMaterials=1},
            new ExpeditionInfo {Area="鎮守府", ID="3", EName="警備任務", Time="00:20:00", Lv=3, ShipNum=3, Exp=30, Fuel=30, Ammunition=30, Steel=40},
            new ExpeditionInfo {Area="鎮守府", ID="4", EName="対潜警戒任務", Time="00:50:00", Lv=3, ShipNum=3, Exp=30, Ammunition=70, InstantRepairMaterials=1, FurnitureBox="小1", RequireShipType= new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> {
                    { new Tuple<string, int>[] {Tuple.Create(LIGHTCRUISER, 1), Tuple.Create(DESTROYER, 2)}, null },
                    { new Tuple<string, int>[] {Tuple.Create(DESTROYER, 1), Tuple.Create(ESCORT, 3)}, null },
                    { new Tuple<string, int>[] {Tuple.Create(ESCORT, 2)}, new Tuple<string, int>[] { Tuple.Create(LIGHTCRUISER, 1), Tuple.Create(TRAININGCRUISER, 1) }},
                    { new Tuple<string, int>[] {Tuple.Create(ESCORTECARRIER, 1)}, new Tuple<string, int>[] { Tuple.Create(DESTROYER, 2), Tuple.Create(ESCORT, 2) }},
                }},
            new ExpeditionInfo {Area="鎮守府", ID="5", EName="海上護衛任務", Time="01:30:00", Lv=3, ShipNum=4, Exp=40, Fuel=200, Ammunition=200, Steel=20, Bauxite=20, RequireShipType = new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> {
                    { new Tuple<string, int>[] {Tuple.Create(LIGHTCRUISER, 1), Tuple.Create(DESTROYER, 2)}, null },
                    { new Tuple<string, int>[] {Tuple.Create(DESTROYER, 1), Tuple.Create(ESCORT, 3)}, null },
                    { new Tuple<string, int>[] {Tuple.Create(ESCORT, 2)}, new Tuple<string, int>[] { Tuple.Create(LIGHTCRUISER, 1), Tuple.Create(TRAININGCRUISER, 1) }},
                    { new Tuple<string, int>[] {Tuple.Create(ESCORTECARRIER, 1)}, new Tuple<string, int>[] { Tuple.Create(DESTROYER, 2), Tuple.Create(ESCORT, 2) }},
                }},
            new ExpeditionInfo {Area="鎮守府", ID="6", EName="防空射撃演習", Time="00:40:00", Lv=4, ShipNum=4, Exp=30, Bauxite=80, FurnitureBox="小1"},
            new ExpeditionInfo {Area="鎮守府", ID="7", EName="観艦式予行", Time="01:00:00", Lv=5,ShipNum=6, Exp=60, Steel=50, Bauxite=30, InstantBuildMaterials=1},
            new ExpeditionInfo {Area="鎮守府", ID="8", EName="観艦式", Time="03:00:00", Lv=6, ShipNum=6, Exp=120, Fuel=50, Ammunition=100, Steel=50, Bauxite=50, InstantBuildMaterials=2, DevelopmentMaterials=1},

            new ExpeditionInfo {Area="鎮守府", ID="A1", EName="兵站強化任務", Time="00:25:00", Lv=5, ShipNum=4, Exp=15, Fuel=45, Ammunition=45, RequireSumShipType=new string[]{ DESTROYER , ESCORT }, RequireSumShipTypeNum = 3 },
            new ExpeditionInfo {Area="鎮守府", ID="A2", EName="海峡警備行動", Time="00:55:00", Lv=20, SumAA=70, SumASW=180, ShipNum=4, Exp=40, Fuel=70, Ammunition=40, Bauxite=10,DevelopmentMaterials=1, InstantRepairMaterials=1, RequireSumShipType=new string[]{ DESTROYER , ESCORT }, RequireSumShipTypeNum = 4 },
            new ExpeditionInfo {Area="鎮守府", ID="A3", EName="長時間対潜警戒", Time="02:15:00", Lv=35,SumLv=185, ShipNum=5, Exp=55, Fuel=120, Steel=60,Bauxite=60, InstantRepairMaterials=1, DevelopmentMaterials=2, RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(LIGHTCRUISER, 1) }, null } }, RequireSumShipType=new string[]{ DESTROYER , ESCORT }, RequireSumShipTypeNum =3, SumASW=280, SumViewRange=60 },
            new ExpeditionInfo {Area="鎮守府", ID="A4", EName="南西方面連絡線哨戒", Time="01:50:00", Lv=40,SumLv=200, ShipNum=5, Exp=45, Fuel=80, Ammunition=120, Bauxite=100, InstantRepairMaterials=2, InstantBuildMaterials=2, RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(LIGHTCRUISER, 1), Tuple.Create(DESTROYER, 2) }, null } }, SumAA=300,SumASW=280,SumViewRange=120 },
            new ExpeditionInfo {Area="鎮守府", ID="A5", EName="小笠原沖哨戒線", Time="03:00:00", Lv=45,SumLv=230, ShipNum=5, Exp=55, Ammunition=300, Bauxite=100, DevelopmentMaterials=4, InstantRepairMaterials=3, RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(LIGHTCRUISER, 1), Tuple.Create(DESTROYER, 3) }, null } }, SumAA=220,SumASW=240,SumViewRange=150 },
            new ExpeditionInfo {Area="鎮守府", ID="A6", EName="小笠原沖戦闘哨戒", Time="03:30:00", Lv=55,SumLv=290, ShipNum=6, Exp=90, Fuel=100, Steel=100, Ammunition=500, Bauxite=200, DevelopmentMaterials=5, AlterationMaterials=1, RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(LIGHTCRUISER, 1), Tuple.Create(DESTROYER, 3) }, null } }, SumAA=300,SumASW=270,SumViewRange=180 },

            new ExpeditionInfo {Area="南西諸島", ID="9", EName="タンカー護衛任務", Time="04:00:00", Lv=3, ShipNum=4, Exp=60, Fuel=350, InstantRepairMaterials=2, FurnitureBox="小1", RequireShipType= new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> {
                    { new Tuple<string, int>[] {Tuple.Create(LIGHTCRUISER, 1)}, new Tuple<string, int>[] { Tuple.Create(DESTROYER, 2)}},
                    { new Tuple<string, int>[] {Tuple.Create(DESTROYER, 1)}, new Tuple<string, int>[] { Tuple.Create(ESCORT, 3) } },
                    { new Tuple<string, int>[] {Tuple.Create(ESCORT, 2)}, new Tuple<string, int>[] { Tuple.Create(LIGHTCRUISER, 1), Tuple.Create(TRAININGCRUISER, 1) }},
                    { new Tuple<string, int>[] {Tuple.Create(ESCORTECARRIER, 1)}, new Tuple<string, int>[] { Tuple.Create(DESTROYER, 2), Tuple.Create(ESCORT, 2) }},
                }},
            new ExpeditionInfo {Area="南西諸島", ID="10", EName="強行偵察任務", Time="01:30:00", Lv=3, ShipNum=3, Exp=40, Ammunition=50, Bauxite=40, InstantRepairMaterials=1, InstantBuildMaterials=1, RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(LIGHTCRUISER, 2) }, null } }, },
            new ExpeditionInfo {Area="南西諸島", ID="11", EName="ボーキサイト輸送任務", Time="05:00:00", Lv=6, ShipNum=4, Exp=40, Bauxite=250, InstantRepairMaterials=1, FurnitureBox="小1", RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(DESTROYER, 2) }, null } }, },
            new ExpeditionInfo {Area="南西諸島", ID="12", EName="資源輸送任務", Time="08:00:00", Lv=4, ShipNum=4, Exp=60, Fuel=50, Ammunition=250, Steel=200, Bauxite=50, DevelopmentMaterials=1, FurnitureBox="中1", RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(DESTROYER, 2) }, null } }, },
            new ExpeditionInfo {Area="南西諸島", ID="13", EName="鼠輸送作戦", Time="04:00:00", Lv=5, ShipNum=6, Exp=70, Fuel=240, Ammunition=300, InstantRepairMaterials=2, FurnitureBox="小1", RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(LIGHTCRUISER, 1), Tuple.Create(DESTROYER, 4) }, null } }, },
            new ExpeditionInfo {Area="南西諸島", ID="14", EName="包囲陸戦隊撤収作戦", Time="06:00:00", Lv=6, ShipNum=6, Exp=90, Ammunition=280, Steel=200, InstantRepairMaterials=1, DevelopmentMaterials=1, RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(LIGHTCRUISER, 1) }, new Tuple<string, int>[] { Tuple.Create(DESTROYER, 3) } } }, },
            new ExpeditionInfo {Area="南西諸島", ID="15", EName="囮機動部隊支援作戦", Time="12:00:00", Lv=8, ShipNum=6, Exp=100, Steel=300, Bauxite=400, DevelopmentMaterials=1, FurnitureBox="大1", RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(AIRCRAFTCARRIER, 2) }, new Tuple<string, int>[] { Tuple.Create(DESTROYER, 2) } } }, },
            new ExpeditionInfo {Area="南西諸島", ID="16", EName="艦隊決戦援護作戦", Time="15:00:00", Lv=10, ShipNum=6, Exp=120, Fuel=500, Ammunition=500, Steel=200, Bauxite=200, InstantBuildMaterials=2, DevelopmentMaterials=2, RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(LIGHTCRUISER, 1) }, new Tuple<string, int>[] { Tuple.Create(DESTROYER, 2) } } }, },

            new ExpeditionInfo {Area="南西諸島", ID="B1", EName="南西方面航空偵察作戦", Time="00:35:00", Lv=40,SumLv=150, SumAA=200,SumASW=200,SumViewRange=140, ShipNum=6, Exp=35, Steel=20, Bauxite=30, InstantRepairMaterials=1,FurnitureBox="小1", RequireShipType =new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(SEAPLANETENDER, 1), Tuple.Create(LIGHTCRUISER, 1), Tuple.Create(DESTROYER, 2) }, null } }, },
            new ExpeditionInfo {Area="南西諸島", ID="B2", EName="敵泊地強襲反撃作戦", Time="08:40:00", Lv=45, SumLv=220, SumAA=160,SumASW=160,SumViewRange=140, ShipNum=6, Exp=70, Fuel=300, Ammunition=200, Steel=100, InstantRepairMaterials=1, DevelopmentMaterials=1, RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(HEAVYCRUISER, 1), Tuple.Create(LIGHTCRUISER, 1), Tuple.Create(DESTROYER, 3) }, null } }, },
            new ExpeditionInfo {Area="南西諸島", ID="B3", EName="南西諸島離島哨戒作戦", Time="02:50:00", Lv=50, SumLv=250, SumAA=220,SumASW=220,SumViewRange=190, ShipNum=6, Exp=50, Ammunition=100, Steel=100, Bauxite=180, FurnitureBox="大2", InstantRepairMaterials=2, RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(SEAPLANETENDER, 1), Tuple.Create(LIGHTCRUISER, 1), Tuple.Create(DESTROYER, 4) }, null } }, },
            new ExpeditionInfo {Area="南西諸島", ID="B4", EName="南西諸島離島防衛作戦", Time="07:30:00", Lv=55, SumLv=300, SumAA=280,SumASW=280,SumViewRange=170, ShipNum=6, Exp=60, Steel=1200, Bauxite=650, DevelopmentMaterials=4, AlterationMaterials=1, RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(HEAVYCRUISER, 1), Tuple.Create(LIGHTCRUISER, 1), Tuple.Create(DESTROYER, 2), Tuple.Create(SUBMARINE, 1) }, null } }, },
            new ExpeditionInfo {Area="南西諸島", ID="B5", EName="南西諸島捜索撃滅戦", Time="06:30:00", Lv=60, SumLv=330, SumAA=400,SumASW=285,SumViewRange=385, ShipNum=6, Exp=100, Fuel=500, Ammunition=500, Steel=1000, Bauxite=750, InstantRepairMaterials=4, AlterationMaterials=1, RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(SEAPLANETENDER, 1), Tuple.Create(LIGHTCRUISER, 1), Tuple.Create(DESTROYER, 2) }, null } }, },
            new ExpeditionInfo {Area="南西諸島", ID="B6", EName="精鋭水雷戦隊夜襲", Time="05:50:00", Lv=60, SumLv=330, SumAA=400,SumASW=285,SumViewRange=340, ShipNum=6, Exp=110, Fuel=600, Ammunition=1000, Steel=600, Bauxite=600, DevelopmentMaterials=5, AlterationMaterials=1, RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(LIGHTCRUISER, 1), Tuple.Create(DESTROYER, 5) }, null } }, },

            new ExpeditionInfo {Area="北方", ID="17", EName="敵地偵察作戦", Time="00:45:00", Lv=20, ShipNum=6, Exp=30, Fuel=70, Ammunition=90, Steel=50, RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(LIGHTCRUISER, 1), Tuple.Create(DESTROYER, 3) }, null } }, },
            new ExpeditionInfo {Area="北方", ID="18", EName="航空機輸送作戦", Time="05:00:00", Lv=15, ShipNum=6, Exp=60, Steel=300, Bauxite=150, InstantRepairMaterials=1, RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(AIRCRAFTCARRIER, 3), Tuple.Create(DESTROYER, 2) }, null } }, },
            new ExpeditionInfo {Area="北方", ID="19", EName="北号作戦", Time="06:00:00", Lv=20, ShipNum=6, Exp=60, Fuel=400, Ammunition=50, Steel=50, Bauxite=30, DevelopmentMaterials=1, FurnitureBox="小1", RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(AVIATIONBATTLESHIP, 2), Tuple.Create(DESTROYER, 2) }, null } }, },
            new ExpeditionInfo {Area="北方", ID="20", EName="潜水艦哨戒任務",  Time="02:00:00", Lv=1, ShipNum=2, Exp=40, Steel=150, DevelopmentMaterials=1, FurnitureBox="小1", RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(SUBMARINE, 1), Tuple.Create(LIGHTCRUISER, 1) }, null } }, },
            new ExpeditionInfo {Area="北方", ID="21", EName="北方鼠輸送作戦", Time="02:20:00", Lv=15, SumLv=30, ShipNum=5, Exp=45, Fuel=320, Ammunition=270, FurnitureBox="小1", RequireItemNum=new Dictionary<string, int> { { DRUMCANISTER, 3 } }, RequireItemShipNum=new Dictionary<string, int>() { { DRUMCANISTER, 3 } }, RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(LIGHTCRUISER, 1), Tuple.Create(DESTROYER, 4) }, null } }, },
            new ExpeditionInfo {Area="北方", ID="22", EName="艦隊演習", Time="03:00:00", Lv=30, SumLv=45, ShipNum=6, Exp=45, Ammunition=10, RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(HEAVYCRUISER, 1), Tuple.Create(LIGHTCRUISER, 1), Tuple.Create(DESTROYER, 2) }, null } }, },
            new ExpeditionInfo {Area="北方", ID="23", EName="航空戦艦運用演習", Time="04:00:00", Lv=50, SumLv=200, ShipNum=6, Exp=70, Ammunition=50, Bauxite=130, FurnitureBox="中1", RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(AVIATIONBATTLESHIP, 2), Tuple.Create(DESTROYER, 2) }, null } }, },
            new ExpeditionInfo {Area="北方", ID="24", EName="北方航路海上護衛", Time="08:20:00", Lv=50, SumLv=200, ShipNum=6, Exp=65, Fuel=500, Bauxite=150, InstantRepairMaterials=1, DevelopmentMaterials=2, RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(LIGHTCRUISER, 1), Tuple.Create(DESTROYER, 4) }, null } }, FlagShipType = LIGHTCRUISER},

            new ExpeditionInfo {Area="南西", ID="41", EName="ブルネイ泊地沖哨戒", Time="01:00:00", Lv=30,SumLv=100, SumAA=80, SumASW=210, ShipNum=3, Exp=30, Fuel=100, Bauxite=20, DevelopmentMaterials=1, InstantRepairMaterials=1, RequireSumShipType=new string[]{ DESTROYER , ESCORT }, RequireSumShipTypeNum =3 },
            new ExpeditionInfo {Area="南西", ID="42", EName="ミ船団護衛(一号船団)", Time="08:00:00", Lv=45,SumLv=200, ShipNum=4, Exp=60, Fuel=800,Bauxite=200, FurnitureBox="大1", InstantBuildMaterials=3, RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(LIGHTCRUISER, 1), Tuple.Create(DESTROYER, 2) }, null } }, },
            new ExpeditionInfo {Area="南西", ID="43", EName="ミ船団護衛(二号船団)", Time="12:00:00", Lv=55,SumLv=300, SumAA=280,SumASW=280,SumViewRange=170, ShipNum=6, Exp=75, Fuel=2000, Bauxite=400, DevelopmentMaterials=4, AlterationMaterials=1, RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(LIGHTCRUISER, 1), Tuple.Create(DESTROYER, 2) }, null } }, FlagShipType = ESCORTECARRIER},
            new ExpeditionInfo {Area="南西", ID="44", EName="航空装備輸送任務", Time="10:00:00", Lv=35,SumLv=210, SumAA=200,SumASW=200,SumViewRange=150, ShipNum=6, Exp=45, Ammunition=200, Bauxite=800, DevelopmentMaterials=4, FurnitureBox="大2", RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(AIRCRAFTCARRIER, 1), Tuple.Create(SEAPLANETENDER, 1), Tuple.Create(LIGHTCRUISER, 1), Tuple.Create(DESTROYER, 2) }, null } }, FlagShipType = AIRCRAFTCARRIER },
            new ExpeditionInfo {Area="南西", ID="45", EName="ボーキサイト船団護衛", Time="03:20:00", Lv=50,SumLv=240, SumAA=240,SumASW=300,SumViewRange=180, ShipNum=5, Exp=35, Fuel=40, Bauxite=220, FurnitureBox="中1", RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(ESCORTECARRIER, 1) }, null } }, RequireSumShipType=new string[]{ DESTROYER , ESCORT }, RequireSumShipTypeNum =4 },
            new ExpeditionInfo {Area="南西", ID="46", EName="南西海域戦闘哨戒", Time="03:30:00", Lv=60,SumLv=300, SumAA=250,SumASW=220,SumViewRange=190, ShipNum=5, Exp=80, Fuel=300, Steel=150, Bauxite=380, DevelopmentMaterials=3, AlterationMaterials=1, RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(HEAVYCRUISER, 2), Tuple.Create(LIGHTCRUISER, 1), Tuple.Create(DESTROYER, 2) }, null } }, },

            new ExpeditionInfo {Area="西方", ID="25", EName="通商破壊作戦", Time="40:00:00", Lv=25, ShipNum=4, Exp=80, Fuel=900, Steel=500, FurnitureBox="中1", RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(HEAVYCRUISER, 2), Tuple.Create(DESTROYER, 2) }, null } }, },
            new ExpeditionInfo {Area="西方", ID="26", EName="敵母港空襲作戦", Time="80:00:00", Lv=30, ShipNum=4, Exp=150, Bauxite=900, InstantRepairMaterials=3, RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(AIRCRAFTCARRIER, 1), Tuple.Create(LIGHTCRUISER, 1), Tuple.Create(DESTROYER, 2) }, null } }, },
            new ExpeditionInfo {Area="西方", ID="27", EName="潜水艦通商破壊作戦", Time="20:00:00", Lv=1, ShipNum=2, Exp=80, Steel=800, DevelopmentMaterials=2, FurnitureBox="小2", RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(SUBMARINE, 2) }, null } }, },
            new ExpeditionInfo {Area="西方", ID="28", EName="西方海域封鎖作戦", Time="25:00:00", Lv=30, ShipNum=3, Exp=100, Steel=900, Bauxite=350, DevelopmentMaterials=3, FurnitureBox="中2", RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(SUBMARINE, 3) }, null } }, },
            new ExpeditionInfo {Area="西方", ID="29", EName="潜水艦派遣演習", Time="24:00:00", Lv=50, ShipNum=3, Exp=100, Ammunition=50, Bauxite=100, DevelopmentMaterials=1, FurnitureBox="小1", RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(SUBMARINE, 3) }, null } }, },
            new ExpeditionInfo {Area="西方", ID="30", EName="潜水艦派遣作戦", Time="48:00:00", Lv=55, ShipNum=4, Exp=100, Bauxite=100, DevelopmentMaterials=3, RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(SUBMARINE, 4) }, null } }, },
            new ExpeditionInfo {Area="西方", ID="31", EName="海外艦との接触", Time="02:00:00", Lv=60, SumLv=200, ShipNum=4, Exp=50, Ammunition=30, FurnitureBox="小1", RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(SUBMARINE, 4) }, null } }, },
            new ExpeditionInfo {Area="西方", ID="32", EName="遠洋練習航海", Time="24:00:00", Lv=5, ShipNum=3, Exp=300, Fuel=50, Ammunition=50, Steel=50, Bauxite=50, FurnitureBox="大1", DevelopmentMaterials=1, RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(TRAININGCRUISER, 1), Tuple.Create(DESTROYER, 2) }, null } }, FlagShipType = TRAININGCRUISER},

            new ExpeditionInfo {Area="西方", ID="D1", EName="西方海域偵察作戦", Time="02:00:00", Lv=50, SumLv=200, SumAA=240,SumASW=240,SumViewRange=300, ShipNum=6, Exp=35, Ammunition=20, Steel=20, Bauxite=100, InstantRepairMaterials=1, RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(SEAPLANETENDER, 1), Tuple.Create(DESTROYER, 3) }, null } }, FlagShipType = SEAPLANETENDER},
            new ExpeditionInfo {Area="西方", ID="D2", EName="西方潜水艦作戦", Time="10:00:00", Lv=55, SumLv=270, SumAA=80,SumASW=50, ShipNum=5, Exp=70, Steel=400, Bauxite=800, FurnitureBox="大1", RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(SUBMARINETENDER, 1), Tuple.Create(SUBMARINE, 3) }, null } }, FlagShipType = SUBMARINETENDER},
            new ExpeditionInfo {Area="西方", ID="D3", EName="欧州方面友軍との接触", Time="12:00:00", Lv=65, SumLv=350, SumAA=90,SumASW=70,SumViewRange=95, ShipNum=5, Exp=80, Ammunition=800, Steel=500, Bauxite=400, InstantRepairMaterials=3, AlterationMaterials=1, RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(SUBMARINETENDER, 1), Tuple.Create(SUBMARINE, 3) }, null } }, FlagShipType = SUBMARINETENDER},

            new ExpeditionInfo {Area="南方", ID="35", EName="MO作戦", Time="07:00:00", Lv=40, ShipNum=6, Exp=100, Steel=240, Bauxite=280, DevelopmentMaterials=1, FurnitureBox="小2", RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(AIRCRAFTCARRIER, 2), Tuple.Create(HEAVYCRUISER, 1), Tuple.Create(DESTROYER, 1) }, null } }, },
            new ExpeditionInfo {Area="南方", ID="36", EName="水上機基地建設", Time="09:00:00", Lv=30, ShipNum=6, Exp=100, Fuel=480, Steel=200, Bauxite=200, InstantRepairMaterials=1, FurnitureBox="中1", RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(SEAPLANETENDER, 2), Tuple.Create(LIGHTCRUISER, 1), Tuple.Create(DESTROYER, 1) }, null } }, },
            new ExpeditionInfo {Area="南方", ID="37", EName="東京急行", Time="02:45:00", Lv=50, SumLv=200, ShipNum=6, Exp=50, Ammunition=380, Steel=270, FurnitureBox="小1", RequireItemNum=new Dictionary<string, int>() { { DRUMCANISTER, 4 } } , RequireItemShipNum=new Dictionary<string, int>() { { DRUMCANISTER, 3 } }, RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(LIGHTCRUISER, 1), Tuple.Create(DESTROYER, 5) }, null } }, },
            new ExpeditionInfo {Area="南方", ID="38", EName="東京急行(弐)", Time="02:55:00", Lv=65, SumLv=240, ShipNum=6, Exp=50, Fuel=420, Steel=200, FurnitureBox="小1", RequireItemNum=new Dictionary<string, int>() { { DRUMCANISTER, 8 } }, RequireItemShipNum=new Dictionary<string, int>() { { DRUMCANISTER, 4 } }, RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(DESTROYER, 5) }, null } }, },
            new ExpeditionInfo {Area="南方", ID="39", EName="遠洋潜水艦作戦", Time="30:00:00", Lv=3, SumLv=180, ShipNum=5, Exp=130, Steel=300, InstantRepairMaterials=2, FurnitureBox="中1", RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(SUBMARINETENDER, 1), Tuple.Create(SUBMARINE, 4), }, null } }, },
            new ExpeditionInfo {Area="南方", ID="40", EName="水上機前線輸送", Time="06:50:00", Lv=25, SumLv=150, ShipNum=6, Exp=60, Fuel=300, Ammunition=300, Bauxite=100, InstantRepairMaterials=1, FurnitureBox="小3", RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(LIGHTCRUISER, 1), Tuple.Create(SEAPLANETENDER, 2), Tuple.Create(DESTROYER, 2) }, null } }, FlagShipType = LIGHTCRUISER},

            new ExpeditionInfo {Area="南方", ID="E1", EName="ラバウル方面艦隊進出", Time="07:30:00", Lv=55, SumLv=290, SumAA=350,SumASW=330,SumViewRange=250, ShipNum=6, Exp=100, Ammunition=600, Steel=600, Bauxite=1000, FurnitureBox="大2", AlterationMaterials=1, FlagShipType = HEAVYCRUISER, RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(HEAVYCRUISER, 1), Tuple.Create(LIGHTCRUISER, 1), Tuple.Create(DESTROYER, 3) }, null } }, },
            new ExpeditionInfo {Area="南方", ID="E2", EName="強行鼠輸送作戦", Time="03:05:00", Lv=70, SumLv=320, SumAA=240,SumASW=220,SumViewRange=160, ShipNum=5, Exp=80, Ammunition=480, InstantRepairMaterials=2, AlterationMaterials=1, RequireItemNum=new Dictionary<string, int>() { { DRUMCANISTER, 4 } }, RequireItemShipNum=new Dictionary<string, int>() { { DRUMCANISTER, 3 } }, RequireShipType=new Dictionary<Tuple<string, int>[], Tuple<string, int>[]> { { new Tuple<string, int>[] { Tuple.Create(DESTROYER, 5) }, null } }, },
        };

        public ExpeditionInfo()
        {
            for (var i = 2; i <= 4; i++)
            {
                isParameter[i] = new Dictionary<string, bool?>();
                isParameter[i].Add(AA, null);
                isParameter[i].Add(ASW, null);
                isParameter[i].Add(VIEWRANGE, null);
            }
        }

        public void Check()
        {
            isSuccess2 = CheckAll(KanColleClient.Current.Homeport.Organization.Fleets[2]);
            isSuccess3 = CheckAll(KanColleClient.Current.Homeport.Organization.Fleets[3]);
            isSuccess4 = CheckAll(KanColleClient.Current.Homeport.Organization.Fleets[4]);

            CheckParam();
        }

        private void CheckParam()
        {
            for (var i = 2; i <= 4; i++)
            {
                var flags = new Dictionary<string, bool?>();
                flags[AA] = SumAA != null ? SumAACheck(KanColleClient.Current.Homeport.Organization.Fleets[i]) : (bool?)null;
                flags[ASW] = SumASW != null ? SumASWCheck(KanColleClient.Current.Homeport.Organization.Fleets[i]) : (bool?)null;
                flags[VIEWRANGE] = SumViewRange != null ? SumViewRangeCheck(KanColleClient.Current.Homeport.Organization.Fleets[i]) : (bool?)null;

                isParameter[i] = flags;
            }
        }

        public static ExpeditionInfo[] ExpeditionList
        {
            get { return _ExpeditionTable; }
            set { _ExpeditionTable = value; }
        }

        public bool CheckAll(Fleet fleet) => CheckShipNum(fleet) &&
                        FlagshipLvCheck(fleet) &&
                        SumLvCheck(fleet) &&
                        RequireShipTypeCheck(fleet) &&
                        RequireItemCheck(fleet) &&
                        FlagShipTypeCheck(fleet) &&
                        RequireSumShipTypeCheck(fleet) &&
                        SumAACheck(fleet) &&
                        SumASWCheck(fleet) &&
                        SumViewRangeCheck(fleet);

        /// <summary>
        /// 艦数チェック
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private bool CheckShipNum(Fleet fleet) => fleet.Ships.Length >= ShipNum;

        /// <summary>
        /// 旗艦Lvチェック
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private bool FlagshipLvCheck(Fleet fleet)
        {
            if (fleet.Ships.Length == 0) return false;

            return fleet.Ships.First().Level >= Lv;
        }

        /// <summary>
        /// 合計Lvチェック
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private bool SumLvCheck(Fleet fleet)
        {
            if (null == SumLv) return true;

            return fleet.Ships.Select(s => s.Level).Sum() >= SumLv;
        }

        /// <summary>
        /// 艦種取得
        /// </summary>
        /// <param name="ship">艦娘</param>
        /// <returns>艦種</returns>
        private string GetShipType(Ship ship) => ship.Info.Name.StartsWith("大鷹") ? "護衛空母" : ship.Info.ShipType.Name;

        /// <summary>
        /// 艦種チェック
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private bool RequireShipTypeCheck(Fleet fleet)
        {
            if (null == RequireShipType) return true;

            return RequireShipType.Any(x =>
                x.Key.All(y => fleet.Ships.Count(i => Regex.IsMatch(GetShipType(i), y.Item1)) >= y.Item2) &&
                (x.Value == null ? true : x.Value.Any(z => fleet.Ships.Count(j => Regex.IsMatch(z.Item1, GetShipType(j))) >= z.Item2)));
        }

        /// <summary>
        /// 装備のチェック
        /// </summary{
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private bool RequireItemCheck(Fleet fleet)
        {
            if (null == RequireItemNum || null == RequireItemShipNum) return true;
            foreach (var pair in RequireItemShipNum)
            {
                var shipNum = fleet.Ships.Where(
                    ship => ship.EquippedItems.Any(
                        item => pair.Key.Equals(item.Item.Info.Name))).Count();

                if (shipNum < pair.Value)
                {
                    return false;
                }
            }
            foreach (var pair in RequireItemNum)
            {
                var itemNum = fleet.Ships.Select(
                    ship => ship.EquippedItems.Where(
                        item => pair.Key.Equals(item.Item.Info.Name)).Count()).Sum();

                if (itemNum < pair.Value)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 旗艦の艦種チェック
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private bool FlagShipTypeCheck(Fleet fleet)
        {
            if (null == FlagShipType) return true;

            if (0 == fleet.Ships.Length) return false;

            var shiptype = fleet.Ships.First().Info.ShipType;
            var regexp = new Regex(FlagShipType);
            var match = regexp.Match(shiptype.Name);

            return match.Success;
        }

        /// <summary>
        /// 必要合算艦種のチェック（駆または海防合わせて3隻というようなのに使用）
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private bool RequireSumShipTypeCheck(Fleet fleet)
        {
            //必要合算艦種が設定されていない場合は自動成功
            if (null == RequireSumShipType) return true;

            var sum = 0;
            foreach (var siptype in RequireSumShipType)
            {
                var re = new Regex(siptype);
                sum += fleet.Ships
                     .Where(f => re.Match(f.Info.ShipType.Name).Success).Count();

                if (sum >= RequireSumShipTypeNum)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 合計対空値のチェック
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private bool SumAACheck(Fleet fleet) => null == SumAA ? true : fleet.Ships.Sum(s => s.AA.Current + s.EquippedItems.Sum(i => i.Item.Info.AA)) >= SumAA;

        /// <summary>
        /// 合計対潜値のチェック
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private bool SumASWCheck(Fleet fleet)
        {
            if (null == SumASW) return true;

            var not_types = new SlotItemType[] { SlotItemType.水上偵察機, SlotItemType.水上爆撃機, SlotItemType.大型飛行艇 };

            //水偵・水爆・飛行艇の対潜値の合計を取得
            var not_sum_asw = fleet.Ships.Select(
                ship => ship.EquippedItems.Where(item => not_types.Any(t => t == item.Item.Info.Type)   //水偵・水爆・飛行艇の絞込み
                    ).Sum(s => s.Item.Info.ASW)).Sum(); //対潜値の合計

            //すべての装備込み対潜値の合計を取得
            var sum_asw = fleet.Ships.Select(s => s.ASW).Sum();

            //水偵・水爆・飛行艇の対潜値を無効にする
            return sum_asw - not_sum_asw >= SumASW;
        }

        /// <summary>
        /// 合計索敵値のチェック
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private bool SumViewRangeCheck(Fleet fleet) => null == SumViewRange ? true : fleet.Ships.Sum(s => s.ViewRange + s.EquippedItems.Sum(e => e.Item.Info.ViewRange)) >= SumViewRange;
    }
}
