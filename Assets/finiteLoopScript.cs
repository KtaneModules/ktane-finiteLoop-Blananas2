using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class finiteLoopScript : MonoBehaviour {

    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMSelectable ModuleSelectable;

    public SpriteRenderer MazeObj;
    public Sprite[] MazeSprites;
    public GameObject MiddleRoom;
    public GameObject DiagramRoom;
    public GameObject Button;
    public GameObject ButtonGroupObj;
    public GameObject KnobObj;
    public GameObject Panel;
    public GameObject DualityObj;
    public GameObject GoodEnd;
    public GameObject BadEnd;
    public KMSelectable[] Arrows;
    public GameObject[] ArrowObjs;
    public Material[] ColorMats; //azure, dark blue, gray, orange, red, white, yellow
    public TextMesh Coord;
    public TextMesh[] Diagram;
    public KMSelectable[] MiddleDots;
    public GameObject[] MiddleDotObjs;
    public KMSelectable MiddleSubmit;
    public TextMesh MiddleWord;
    public KMSelectable DiagramSwitch;
    public GameObject SwitchObject;
    public KMSelectable HardResetButton;
    public TextMesh ResetText;
    public KMSelectable[] ButtonGroup;
    public GameObject[] ButtonGroupObjs;
    public KMSelectable Knob;
    public KMSelectable[] PanelButtons;
    public GameObject[] PanelButtonObjs;
    public TextMesh PanelDisplay;
    public TextMesh[] EndText; //Index, Key, Cipher, Letter
    public TextMesh LoopNumber;

    public KMSelectable TestButton; //bc testharness go brrr
    private bool InLoop = false;
    Coroutine Loop;
    private float LoopTime = 0f;
    private int ResetCount = 0;
    private List<float> LoopPoints = new List<float> {}; //These are when the events happen.
    private List<string> LoopActions = new List<string> {}; //Which event happened.
    private List<int> LoopRuns = new List<int> {}; //Which run through the loop it occured in.
    private int LoopPointer = 0;
    private int[] LoopObjFlags = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}; //States of objects in the loop. See block below.
    private bool[] LoopDoorFlags = {false, false, false, true, false, false, false, false, false, true, false, false}; 
    private string LoopAnswer = "";

    //Which doors at intersections are open, each set of 3 are doors in the same room.

    /*
    0/1/2/3/4/5 = 0/1, whether dots 1/2/3/4/5/6 are held.
    6 = 0/1, which direction the switch is in: up/down.
    7 = #, how many times hard reset button has been pressed.
    8/9/10 = whether button 1/2/3 in button group are held.
    11 = #, which way the knob is turned
    12/13/14 = 0/1, whether buttons 1/2/3 in panel are held
    15 = sum of 12,13,14!
    */

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    private int mazeIx;
    private string directions = " ˄˂┘˅│┐┤˃└─┴┌├┬┼";
    private string[] mazes = {"┌┐˃┐˅││˅││˄├┼┼┤˅˄˄││└──┘˄", "┌˂˃┬┐└┐˅│˄┌┼┼┤˅│˄˄││└─˂└┘", "˃┬˂┌┐˅│˅│││├┼┤│││˄│˄└┘˃┴˂", "┌──┐˅˄˅˅││˃┼┼┼┤˅˄˄││└──┘˄", "┌┬˂˃┐││˅┌┘˄├┼┤˅┌┘˄││└˂˃┴┘", "˅┌˂┌┐├┤˅││˄├┼┤˄˅│˄└┐└┘˃─┘", "┌─˂˃┐└┐˅┌┘˃┼┼┼˂┌┘˄└┐└˂˃─┘", "┌˂˃─┐├┐˅┌┘˄├┼┤˅┌┘˄└┤└─˂˃┘"};
    private string[] origins = {"...........r.ll..........", "...d.......r.l...........", ".d.........r.l.........u.", "...........r.ll..........", ".d.........r.l.........u.", ".....rd....r.l...........", "...........r.l...........", ".....r.....r.l.....l....."};
    private string curMaze;
    private string curOri;
    private int curPos = 12;
    private string interestingMaze = "";
    private string[] coordSet = {"A1", "B1", "C1", "D1", "E1", "A2", "B2", "C2", "D2", "E2", "A3", "B3", "C3", "D3", "E3", "A4", "B4", "C4", "D4", "E4", "A5", "B5", "C5", "D5", "E5" };
    private int offset;
    private string[] ciphers = {"A1Z26", "CAESAR", "ATBASH", "AFFINE", "VIGENÈRE", "MODERN"};
    private List<string> words = new List<string> { "ABOARD", "ABROAD", "ABSORB", "ABUSED", "ABUSES", "ACCENT", "ACCEPT", "ACCESS", "ACCORD", "ACROSS", "ACTING", "ACTION", "ACTORS", "ADDING", "ADJUST", "ADMIRE", "ADMITS", "ADULTS", "ADVENT", "ADVERT", "ADVICE", "ADVISE", "AFFAIR", "AFFECT", "AFFORD", "AFIELD", "AGEING", "AGENCY", "AGENDA", "AGENTS", "AGREED", "AGREES", "AIMING", "ALBEIT", "ALBUMS", "ALIENS", "ALLIES", "ALLOWS", "ALMOST", "ALWAYS", "AMIDST", "AMOUNT", "AMUSED", "ANCHOR", "ANGELS", "ANGLES", "ANIMAL", "ANKLES", "ANSWER", "ANYHOW", "ANYONE", "ANYWAY", "APPEAL", "APPEAR", "APPLES", "ARCHES", "ARGUED", "ARGUES", "ARISEN", "ARISES", "ARMIES", "ARMOUR", "AROUND", "ARREST", "ARRIVE", "ARROWS", "ARTERY", "ARTIST", "ASCENT", "ASHORE", "ASKING", "ASPECT", "ASSENT", "ASSERT", "ASSESS", "ASSETS", "ASSIGN", "ASSIST", "ASSUME", "ASSURE", "ASTHMA", "ASYLUM", "ATTACH", "ATTACK", "ATTAIN", "ATTEND", "AUTHOR", "AUTUMN", "AVENUE", "AVOIDS", "AWARDS", "BABIES", "BACKED", "BALLET", "BALLOT", "BANANA", "BANGED", "BANKER", "BANNED", "BANNER", "BARELY", "BARLEY", "BARMAN", "BARONS", "BARREL", "BASICS", "BASINS", "BASKET", "BATTLE", "BEASTS", "BEATEN", "BEAUTY", "BECAME", "BECOME", "BEFORE", "BEGGED", "BEGINS", "BEHALF", "BEHAVE", "BEHIND", "BEINGS", "BELIEF", "BELONG", "BESIDE", "BETTER", "BEWARE", "BEYOND", "BIDDER", "BIGGER", "BIOPSY", "BIRTHS", "BISHOP", "BITING", "BITTEN", "BLACKS", "BLADES", "BLAMED", "BLOCKS", "BLOKES", "BLOODY", "BLOUSE", "BOARDS", "BOASTS", "BODIES", "BOILER", "BOLDLY", "BOMBER", "BONNET", "BOOKED", "BORDER", "BORROW", "BOSSES", "BOTHER", "BOTTLE", "BOTTOM", "BOUGHT", "BOUNDS", "BOWLER", "BOXING", "BRAINS", "BRAKES", "BRANCH", "BRANDS", "BRANDY", "BREACH", "BREAKS", "BREAST", "BREATH", "BREEDS", "BREEZE", "BRICKS", "BRIDGE", "BRINGS", "BROKEN", "BROKER", "BRONZE", "BUBBLE", "BUCKET", "BUDGET", "BUFFER", "BUFFET", "BUGGER", "BUILDS", "BULLET", "BUNDLE", "BURDEN", "BUREAU", "BURIAL", "BURIED", "BURNED", "BURROW", "BURSTS", "BUSHES", "BUTLER", "BUTTER", "BUTTON", "BUYERS", "BUYING", "BYPASS", "CABLES", "CALLED", "CALLER", "CALMLY", "CALVES", "CAMERA", "CAMPUS", "CANALS", "CANCEL", "CANCER", "CANDLE", "CANNON", "CANOPY", "CANVAS", "CARBON", "CAREER", "CARERS", "CARING", "CARPET", "CARROT", "CARVED", "CASTLE", "CATTLE", "CAUGHT", "CAUSED", "CAUSES", "CAVITY", "CEASED", "CELLAR", "CEMENT", "CENSUS", "CENTER", "CENTRE", "CEREAL", "CHAINS", "CHAIRS", "CHANCE", "CHANGE", "CHAPEL", "CHARGE", "CHARTS", "CHECKS", "CHEEKS", "CHEERS", "CHEESE", "CHEQUE", "CHERRY", "CHICKS", "CHIEFS", "CHOICE", "CHOOSE", "CHORDS", "CHORUS", "CHOSEN", "CHUNKS", "CHURCH", "CINEMA", "CIRCLE", "CIRCUS", "CITIES", "CITING", "CLAIMS", "CLAUSE", "CLERGY", "CLERKS", "CLIENT", "CLIFFS", "CLIMAX", "CLINIC", "CLOCKS", "CLONES", "CLOSED", "CLOSER", "CLOSES", "CLOUDS", "CLUTCH", "COASTS", "COFFEE", "COFFIN", "COHORT", "COLDER", "COLDLY", "COLLAR", "COLONY", "COLOUR", "COLUMN", "COMBAT", "COMEDY", "COMING", "COMMIT", "COMPLY", "CONVEY", "CONVOY", "COOKED", "COOKER", "COOLER", "COOLLY", "COPIED", "COPIES", "COPING", "COPPER", "CORNER", "CORPSE", "CORPUS", "CORTEX", "COTTON", "COUNTS", "COUNTY", "COUPLE", "COUPON", "COURSE", "COURTS", "COUSIN", "COVERS", "CRACKS", "CRADLE", "CREATE", "CREDIT", "CRIMES", "CRISES", "CRISIS", "CRISPS", "CRITIC", "CROWDS", "CRUISE", "CRYING", "CUCKOO", "CURLED", "CURSED", "CURSOR", "CURVES", "CUSTOM", "CUTTER", "CYCLES", "DAMAGE", "DANCED", "DANCER", "DANCES", "DANGER", "DARING", "DARKER", "DASHED", "DATING", "DEALER", "DEARLY", "DEATHS", "DEBATE", "DEBRIS", "DEBTOR", "DECADE", "DECIDE", "DECREE", "DEEMED", "DEEPER", "DEEPLY", "DEFEAT", "DEFECT", "DEFEND", "DEFINE", "DEGREE", "DELAYS", "DEMAND", "DEMISE", "DEMONS", "DENIAL", "DENIED", "DENIES", "DEPEND", "DEPTHS", "DEPUTY", "DERIVE", "DESERT", "DESIGN", "DESIRE", "DETAIL", "DETECT", "DEVICE", "DEVISE", "DEVOTE", "DIESEL", "DIFFER", "DIGEST", "DIGITS", "DINGHY", "DINING", "DINNER", "DIRECT", "DISHES", "DISMAY", "DIVERS", "DIVERT", "DIVIDE", "DIVING", "DOCTOR", "DOLLAR", "DOMAIN", "DONKEY", "DONORS", "DOOMED", "DOUBLE", "DOUBLY", "DOUBTS", "DOZENS", "DRAGON", "DRAINS", "DRAWER", "DREAMS", "DRINKS", "DRIVEN", "DRIVER", "DRIVES", "DRYING", "DUMPED", "DURING", "DUTIES", "EAGLES", "EARNED", "EASIER", "EASILY", "EASING", "EASTER", "EATING", "ECHOED", "ECHOES", "EDITED", "EDITOR", "EFFECT", "EFFORT", "EIGHTH", "EIGHTY", "EITHER", "ELBOWS", "ELDERS", "ELDEST", "ELEVEN", "ELITES", "EMBARK", "EMBRYO", "EMERGE", "EMPIRE", "EMPLOY", "ENABLE", "ENAMEL", "ENDING", "ENDURE", "ENERGY", "ENGAGE", "ENGINE", "ENJOYS", "ENOUGH", "ENSURE", "ENTAIL", "ENTERS", "ENTITY", "ENZYME", "EQUALS", "EQUITY", "ERODED", "ERRORS", "ESCAPE", "ESCORT", "ESSAYS", "ESTATE", "ESTEEM", "ETHICS", "EVENLY", "EVENTS", "EVOLVE", "EXCEED", "EXCEPT", "EXCESS", "EXCUSE", "EXISTS", "EXPAND", "EXPECT", "EXPERT", "EXPIRY", "EXPORT", "EXPOSE", "EXTEND", "EXTENT", "EXTRAS", "FABRIC", "FACETS", "FACING", "FACTOR", "FADING", "FAILED", "FAIRLY", "FALLEN", "FAMILY", "FAMINE", "FARMER", "FASTER", "FATHER", "FAULTS", "FAVOUR", "FEARED", "FELLOW", "FEMALE", "FENCES", "FIBRES", "FIELDS", "FIGHTS", "FIGURE", "FILLED", "FILTER", "FINALE", "FINALS", "FINELY", "FINEST", "FINGER", "FINISH", "FIRING", "FIRMLY", "FITTED", "FIXING", "FLAMES", "FLANKS", "FLATLY", "FLIGHT", "FLOCKS", "FLOODS", "FLOORS", "FLOWED", "FLOWER", "FLUIDS", "FLURRY", "FLYING", "FOLDED", "FOLDER", "FOLLOW", "FORCED", "FORCES", "FOREST", "FORGET", "FORGOT", "FORMAT", "FORMED", "FORMER", "FOSSIL", "FOSTER", "FOUGHT", "FOURTH", "FRAMES", "FRANCS", "FREELY", "FREEZE", "FRENCH", "FRENZY", "FRIDGE", "FRIEND", "FRIGHT", "FRINGE", "FRONTS", "FROZEN", "FRUITS", "FULFIL", "FULLER", "FUNDED", "FUNGUS", "FUSION", "FUTURE", "GAINED", "GALAXY", "GALLON", "GAMBLE", "GARAGE", "GARDEN", "GARLIC", "GASPED", "GATHER", "GAZING", "GEARED", "GENDER", "GENIUS", "GENTLY", "GENTRY", "GERMAN", "GHOSTS", "GIANTS", "GIVING", "GLADLY", "GLANCE", "GLANDS", "GLARED", "GLIDER", "GLOVES", "GOLFER", "GOSPEL", "GOSSIP", "GOVERN", "GRADES", "GRAINS", "GRANNY", "GRANTS", "GRAPES", "GRAPHS", "GRAVEL", "GRAVES", "GREASE", "GREENS", "GRIMLY", "GROOVE", "GROUND", "GROUPS", "GROWTH", "GUARDS", "GUESTS", "GUIDED", "GUIDES", "GUITAR", "GUNMEN", "GUTTER", "HABITS", "HALTED", "HALVES", "HAMLET", "HAMMER", "HANDED", "HANDLE", "HAPPEN", "HARDER", "HARDLY", "HASSLE", "HATRED", "HAULED", "HAVING", "HAZARD", "HEADED", "HEADER", "HEALTH", "HEARTH", "HEARTS", "HEATER", "HEAVED", "HEAVEN", "HEDGES", "HEIGHT", "HELMET", "HELPED", "HELPER", "HEROES", "HIDDEN", "HIDING", "HIGHER", "HIGHLY", "HINDER", "HISSED", "HOCKEY", "HOLDER", "HOMAGE", "HONOUR", "HOPING", "HORROR", "HORSES", "HOSTEL", "HOTELS", "HOUNDS", "HOUSED", "HOUSES", "HUGELY", "HUGGED", "HUMANS", "HUMOUR", "HUNGER", "HUNTER", "HURDLE", "IDEALS", "IGNORE", "IMAGES", "IMPACT", "IMPORT", "IMPOSE", "INCHES", "INCOME", "INDEED", "INDUCE", "INFANT", "INFLUX", "INFORM", "INJURY", "INLAND", "INPUTS", "INSECT", "INSIDE", "INSIST", "INSULT", "INSURE", "INTAKE", "INTEND", "INTENT", "INVENT", "INVEST", "INVITE", "ISLAND", "ISSUED", "ISSUES", "ITSELF", "JACKET", "JAILED", "JARGON", "JERKED", "JERSEY", "JEWELS", "JOCKEY", "JOINED", "JOINTS", "JOKING", "JUDGED", "JUDGES", "JUMBLE", "JUMPED", "JUMPER", "JUNGLE", "KEENLY", "KEEPER", "KETTLE", "KICKED", "KIDNEY", "KILLED", "KILLER", "KINDLY", "KISSED", "KISSES", "KNIGHT", "KNIVES", "LABELS", "LABOUR", "LACKED", "LADDER", "LADIES", "LANDED", "LARGER", "LARVAE", "LASHES", "LASTED", "LASTLY", "LATELY", "LATEST", "LATTER", "LAUGHS", "LAUNCH", "LAWYER", "LAYERS", "LAYING", "LAYOUT", "LEADER", "LEAGUE", "LEANED", "LEARNS", "LEARNT", "LEASES", "LEAVES", "LEGACY", "LEGEND", "LEGION", "LENDER", "LENGTH", "LENSES", "LESION", "LESSON", "LETTER", "LEVELS", "LICKED", "LIFTED", "LIGHTS", "LIKING", "LIMITS", "LINING", "LINKED", "LIQUID", "LIQUOR", "LISTED", "LISTEN", "LITRES", "LITTER", "LITTLE", "LIVING", "LOADED", "LOCALS", "LOCATE", "LOCKED", "LODGED", "LONGED", "LONGER", "LOOKED", "LOSERS", "LOSING", "LOSSES", "LOUDER", "LOUDLY", "LOUNGE", "LOVERS", "LOVING", "LOWEST", "LUXURY", "LYRICS", "MAGNET", "MAINLY", "MAKERS", "MAKING", "MALICE", "MAMMAL", "MANAGE", "MANNER", "MANTLE", "MANUAL", "MARBLE", "MARGIN", "MARKED", "MARKER", "MARKET", "MARROW", "MASSES", "MASTER", "MATRIX", "MATTER", "MEADOW", "MEDALS", "MEDIUM", "MELODY", "MEMBER", "MEMORY", "MENACE", "MERELY", "MERGER", "MERITS", "METALS", "METHOD", "METRES", "MIDDAY", "MIDDLE", "MILDLY", "MINERS", "MINING", "MINUTE", "MIRROR", "MISERY", "MISSED", "MISSES", "MISUSE", "MIXING", "MOANED", "MODELS", "MODIFY", "MODULE", "MOMENT", "MONIES", "MONKEY", "MONTHS", "MORALE", "MORALS", "MORTAR", "MOSAIC", "MOSQUE", "MOSTLY", "MOTHER", "MOTIFS", "MOTION", "MOTIVE", "MOTORS", "MOUTHS", "MOVIES", "MOVING", "MUCOSA", "MURDER", "MURMUR", "MUSCLE", "MUSEUM", "MUSTER", "MYRIAD", "MYSELF", "NAMELY", "NATION", "NATURE", "NEARBY", "NEARER", "NEARLY", "NEATLY", "NEEDED", "NEEDLE", "NEPHEW", "NERVES", "NEWEST", "NICELY", "NIGHTS", "NINETY", "NOBLES", "NOBODY", "NODDED", "NOISES", "NOTICE", "NOTIFY", "NOTING", "NOTION", "NOUGHT", "NOVELS", "NOVICE", "NUCLEI", "NUMBER", "NURSES", "OBJECT", "OBTAIN", "OCCUPY", "OCCURS", "OCEANS", "OFFERS", "OFFICE", "OFFSET", "OLDEST", "ONIONS", "OPENED", "OPENER", "OPENLY", "OPERAS", "OPPOSE", "OPTING", "OPTION", "ORANGE", "ORDEAL", "ORDERS", "ORGANS", "ORIGIN", "OTHERS", "OUTFIT", "OUTING", "OUTLET", "OUTPUT", "OUTSET", "OWNERS", "OXYGEN", "PACKED", "PACKET", "PALACE", "PANELS", "PAPERS", "PARADE", "PARCEL", "PARDON", "PARENT", "PARISH", "PARITY", "PARKED", "PARROT", "PARTED", "PARTLY", "PASSED", "PASSES", "PASTRY", "PATENT", "PATROL", "PATRON", "PATTED", "PAUSED", "PAVING", "PAYERS", "PAYING", "PEARLS", "PEERED", "PENCIL", "PEOPLE", "PEPPER", "PERIOD", "PERMIT", "PERSON", "PETALS", "PETROL", "PHASES", "PHONED", "PHONES", "PHOTOS", "PHRASE", "PICKED", "PICNIC", "PIECES", "PIGEON", "PILLAR", "PILLOW", "PILOTS", "PIRATE", "PISTOL", "PLACED", "PLACES", "PLAGUE", "PLAINS", "PLANES", "PLANET", "PLANTS", "PLAQUE", "PLASMA", "PLATES", "PLAYED", "PLAYER", "PLEASE", "PLEDGE", "PLENTY", "PLIGHT", "POCKET", "POETRY", "POINTS", "POISON", "POLICE", "POLICY", "POLISH", "POLLEN", "PONIES", "POORER", "POORLY", "POPPED", "PORTAL", "PORTER", "POSING", "POSTED", "POSTER", "POTATO", "POUNDS", "POURED", "POWDER", "POWERS", "PRAISE", "PRAYED", "PRAYER", "PREFER", "PRETTY", "PRICED", "PRICES", "PRIEST", "PRINCE", "PRINTS", "PRISON", "PRIZES", "PROBES", "PROFIT", "PROVED", "PROVES", "PUBLIC", "PULLED", "PULSES", "PUNISH", "PUPILS", "PURELY", "PURITY", "PURSUE", "PUSHED", "PUZZLE", "PYLORI", "QUARRY", "QUOTAS", "QUOTED", "QUOTES", "RABBIT", "RACING", "RACISM", "RACIST", "RACKET", "RADIOS", "RADIUS", "RAISED", "RAISES", "RANGED", "RANGES", "RARELY", "RARITY", "RATHER", "RATING", "RATIOS", "READER", "REALLY", "REASON", "REBELS", "RECALL", "RECIPE", "RECKON", "RECORD", "RECTOR", "REDUCE", "REFERS", "REFLEX", "REFLUX", "REFORM", "REFUGE", "REFUSE", "REGAIN", "REGARD", "REGIME", "REGION", "REGRET", "REJECT", "RELATE", "RELICS", "RELIED", "RELIEF", "RELIES", "REMAIN", "REMARK", "REMEDY", "REMIND", "REMOVE", "RENDER", "RENTAL", "REPAID", "REPAIR", "REPEAT", "REPLAY", "REPORT", "RESCUE", "RESIGN", "RESIST", "RESORT", "RESTED", "RESULT", "RESUME", "RETAIN", "RETINA", "RETIRE", "RETURN", "REVEAL", "REVERT", "REVIEW", "REVISE", "REVIVE", "REVOLT", "REWARD", "RHYTHM", "RIBBON", "RICHER", "RICHLY", "RIDDEN", "RIDERS", "RIDGES", "RIDING", "RIFLES", "RIGHTS", "RIPPED", "RISING", "RITUAL", "RIVALS", "RIVERS", "ROARED", "ROBOTS", "ROCKET", "ROLLED", "ROLLER", "ROMANS", "ROOTED", "ROUNDS", "ROUTES", "RUBBED", "RUBBER", "RUBBLE", "RUINED", "RULERS", "RULING", "RUMOUR", "RUNNER", "RUNWAY", "RUSHED", "SACKED", "SADDLE", "SAFELY", "SAFEST", "SAFETY", "SAILED", "SAILOR", "SAINTS", "SALADS", "SALARY", "SALMON", "SALOON", "SAMPLE", "SAUCER", "SAVING", "SAYING", "SCALES", "SCENES", "SCHEME", "SCHOOL", "SCORED", "SCORER", "SCORES", "SCOUTS", "SCRAPS", "SCREAM", "SCREEN", "SCREWS", "SCRIPT", "SEALED", "SEAMEN", "SEARCH", "SEASON", "SEATED", "SECOND", "SECRET", "SECTOR", "SECURE", "SEEING", "SEEMED", "SEIZED", "SELDOM", "SELECT", "SELLER", "SENATE", "SENSED", "SENSES", "SERIES", "SERMON", "SERVED", "SERVER", "SERVES", "SETTEE", "SETTLE", "SEVENS", "SEWAGE", "SEWING", "SEXISM", "SEXIST", "SHADES", "SHADOW", "SHAFTS", "SHAKEN", "SHAPED", "SHAPES", "SHARED", "SHARES", "SHEETS", "SHELLS", "SHERRY", "SHIELD", "SHIFTS", "SHIRTS", "SHOCKS", "SHORES", "SHORTS", "SHOULD", "SHOUTS", "SHOWED", "SHOWER", "SHRINE", "SHRUBS", "SIGHED", "SIGHTS", "SIGNAL", "SIGNED", "SILVER", "SIMPLY", "SINGER", "SIPPED", "SISTER", "SKETCH", "SKILLS", "SKIRTS", "SLAVES", "SLEEVE", "SLICES", "SLIDES", "SLOGAN", "SLOPES", "SLOWED", "SLOWER", "SLOWLY", "SMELLS", "SMILED", "SMILES", "SMOKED", "SNAKES", "SOCCER", "SOCKET", "SODIUM", "SOFTEN", "SOFTER", "SOFTLY", "SOLELY", "SOLIDS", "SOLVED", "SOONER", "SORROW", "SORTED", "SOUGHT", "SOUNDS", "SOURCE", "SOVIET", "SPACES", "SPARED", "SPEAKS", "SPEECH", "SPEEDS", "SPELLS", "SPENDS", "SPHERE", "SPIDER", "SPINES", "SPIRAL", "SPIRIT", "SPLASH", "SPOKEN", "SPONGE", "SPORTS", "SPOUSE", "SPRANG", "SPREAD", "SPRING", "SQUADS", "SQUARE", "SQUASH", "STABLE", "STAGED", "STAGES", "STAIRS", "STAKES", "STALLS", "STAMPS", "STANCE", "STANDS", "STAPLE", "STARED", "STARTS", "STATED", "STATES", "STATUE", "STATUS", "STAYED", "STENCH", "STEREO", "STICKS", "STITCH", "STOCKS", "STOLEN", "STONES", "STORED", "STORES", "STOREY", "STORMS", "STRAIN", "STRAND", "STRAPS", "STRATA", "STREAK", "STREAM", "STREET", "STRESS", "STRIDE", "STRIKE", "STRING", "STRIPS", "STRODE", "STROKE", "STROLL", "STRUCK", "STUDIO", "STYLES", "SUBMIT", "SUBURB", "SUCKED", "SUFFER", "SUITED", "SUMMED", "SUMMER", "SUMMIT", "SUMMON", "SUNSET", "SUPPER", "SUPPLY", "SURELY", "SURVEY", "SWEETS", "SWITCH", "SWORDS", "SYMBOL", "SYNTAX", "SYSTEM", "TABLES", "TABLET", "TACKLE", "TACTIC", "TAILOR", "TAKING", "TALENT", "TALKED", "TALLER", "TANGLE", "TANKER", "TAPPED", "TARGET", "TARIFF", "TARMAC", "TASTED", "TASTES", "TAUGHT", "TEASED", "TEMPER", "TEMPLE", "TENANT", "TENDED", "TENNIS", "TENURE", "TERMED", "TERROR", "TESTED", "THANKS", "THEIRS", "THEMES", "THEORY", "THESES", "THESIS", "THIGHS", "THINGS", "THINKS", "THINLY", "THIRDS", "THIRTY", "THOUGH", "THREAD", "THREAT", "THRILL", "THROAT", "THRONE", "THROWN", "THROWS", "THRUST", "TICKET", "TIGERS", "TIGHTS", "TILLER", "TIMBER", "TIMING", "TIPPED", "TISSUE", "TITLES", "TOILET", "TOKENS", "TOMATO", "TONGUE", "TONNES", "TOPICS", "TOPPED", "TORIES", "TORQUE", "TOSSED", "TOWARD", "TOWELS", "TOWERS", "TRACED", "TRACES", "TRACKS", "TRACTS", "TRADER", "TRADES", "TRAINS", "TRAITS", "TRAUMA", "TRAVEL", "TREATS", "TREATY", "TRENCH", "TRENDS", "TRIALS", "TRIBES", "TRICKS", "TROOPS", "TROPHY", "TRUCKS", "TRUSTS", "TRUTHS", "TRYING", "TUCKED", "TUGGED", "TUMOUR", "TUNNEL", "TURKEY", "TURNED", "TUTORS", "TWELVE", "TWENTY", "TYPING", "ULCERS", "UNDULY", "UNEASE", "UNIONS", "UNITED", "UNLESS", "UNLIKE", "UNREST", "UPDATE", "UPHELD", "UPLAND", "UPTAKE", "URGING", "VACUUM", "VALLEY", "VALUED", "VALUES", "VALVES", "VANITY", "VAPOUR", "VARIED", "VARIES", "VASTLY", "VELVET", "VENDOR", "VENUES", "VERSES", "VERSUS", "VESSEL", "VICTIM", "VIDEOS", "VIEWED", "VIEWER", "VIGOUR", "VILLAS", "VIOLIN", "VIRTUE", "VISION", "VISITS", "VOICES", "VOLUME", "VOTERS", "VOTING", "VOWELS", "VOYAGE", "WAGONS", "WAITED", "WAITER", "WAKING", "WALKED", "WALLET", "WALNUT", "WANDER", "WANTED", "WARDEN", "WARILY", "WARMER", "WARMLY", "WARMTH", "WARNED", "WASHED", "WASTED", "WASTES", "WATERS", "WAVING", "WEAKEN", "WEAKER", "WEAKLY", "WEALTH", "WEAPON", "WEEKLY", "WEIGHT", "WHALES", "WHEELS", "WHILST", "WHISKY", "WHITES", "WHOLLY", "WICKET", "WIDELY", "WIDEST", "WIDOWS", "WILDLY", "WINDOW", "WINGER", "WINNER", "WINTER", "WIPING", "WIRING", "WISDOM", "WISELY", "WISHED", "WISHES", "WITHIN", "WIZARD", "WOLVES", "WONDER", "WORKED", "WORKER", "WORLDS", "WOUNDS", "WRISTS", "WRITER", "WRITES", "YACHTS", "YELLED", "YIELDS", "YOUTHS" };
    private int wordIx;
    private string chosenWord;
    private string[] endKeys = { "", "", "", "", "", "" };
    private string[] endLetters = { "", "", "", "", "", "" };
    private string[] diagramTexts = { "" , "" };
    private bool areWeAtAnIntersection = false;
    private string[] damnDoors = {"", "", "", ""};

    void Awake () {
        moduleId = moduleIdCounter++;
        
        ModuleSelectable.OnFocus += delegate () { StartLoop(); };
        ModuleSelectable.OnDefocus += delegate () { if (InLoop) { SoftReset(); } };

        TestButton.OnInteract += delegate () { TestPress(); return false; };
        
        foreach (KMSelectable Arrow in Arrows) {
            Arrow.OnInteract += delegate () { ArrowPress(Arrow); return false; };
        }

        MiddleDots[0].OnInteract += delegate () { PressAnything(true, "DOT 1 HOLD"); return false; };
        MiddleDots[0].OnInteractEnded += delegate () { PressAnything(true, "DOT 1 RELEASE"); };
        MiddleDots[1].OnInteract += delegate () { PressAnything(true, "DOT 2 HOLD"); return false; };
        MiddleDots[1].OnInteractEnded += delegate () { PressAnything(true, "DOT 2 RELEASE"); };
        MiddleDots[2].OnInteract += delegate () { PressAnything(true, "DOT 3 HOLD"); return false; };
        MiddleDots[2].OnInteractEnded += delegate () { PressAnything(true, "DOT 3 RELEASE"); };
        MiddleDots[3].OnInteract += delegate () { PressAnything(true, "DOT 4 HOLD"); return false; };
        MiddleDots[3].OnInteractEnded += delegate () { PressAnything(true, "DOT 4 RELEASE"); };
        MiddleDots[4].OnInteract += delegate () { PressAnything(true, "DOT 5 HOLD"); return false; };
        MiddleDots[4].OnInteractEnded += delegate () { PressAnything(true, "DOT 5 RELEASE"); };
        MiddleDots[5].OnInteract += delegate () { PressAnything(true, "DOT 6 HOLD"); return false; };
        MiddleDots[5].OnInteractEnded += delegate () { PressAnything(true, "DOT 6 RELEASE"); };
        MiddleSubmit.OnInteract += delegate () { PressAnything(true, "SUBMIT PRESS"); return false; };
        DiagramSwitch.OnInteract += delegate () { PressAnything(true, "SWITCH TOGGLE"); return false; };
        HardResetButton.OnInteract += delegate () { PressAnything(true, "HARD RESET PRESS"); return false; };
        ButtonGroup[0].OnInteract += delegate () { PressAnything(true, "GROUP BUTTON 1 HOLD"); return false; };
        ButtonGroup[0].OnInteractEnded += delegate () { PressAnything(true, "GROUP BUTTON 1 RELEASE"); };
        ButtonGroup[1].OnInteract += delegate () { PressAnything(true, "GROUP BUTTON 2 HOLD"); return false; };
        ButtonGroup[1].OnInteractEnded += delegate () { PressAnything(true, "GROUP BUTTON 2 RELEASE"); };
        ButtonGroup[2].OnInteract += delegate () { PressAnything(true, "GROUP BUTTON 3 HOLD"); return false; };
        ButtonGroup[2].OnInteractEnded += delegate () { PressAnything(true, "GROUP BUTTON 3 RELEASE"); };
        Knob.OnInteract += delegate () { PressAnything(true, "KNOB TURN"); return false; };
        PanelButtons[0].OnInteract += delegate () { PressAnything(true, "PANEL BUTTON 1 HOLD"); return false; };
        PanelButtons[0].OnInteractEnded += delegate () { PressAnything(true, "PANEL BUTTON 1 RELEASE"); };
        PanelButtons[1].OnInteract += delegate () { PressAnything(true, "PANEL BUTTON 2 HOLD"); return false; };
        PanelButtons[1].OnInteractEnded += delegate () { PressAnything(true, "PANEL BUTTON 2 RELEASE"); };
        PanelButtons[2].OnInteract += delegate () { PressAnything(true, "PANEL BUTTON 3 HOLD"); return false; };
        PanelButtons[2].OnInteractEnded += delegate () { PressAnything(true, "PANEL BUTTON 3 RELEASE"); };
    }

    // Use this for initialization
    void Start () {
        mazeIx = UnityEngine.Random.Range(0,8);
        curMaze = mazes[mazeIx];
        MazeObj.sprite = MazeSprites[mazeIx+1];
        offset = UnityEngine.Random.Range(0,4);
        MakeMazeInteresting();
        GenerateCipher();
        ShowRoom();
    }

    void MakeMazeInteresting() {
        bool skipSecond = false;
        int ends = 0;
        List<int> endLocations = new List<int> {};
        List<int> badEnds = new List<int> {};
        int intersections = 0;
        List<int> interLocations = new List<int> {};
        string num = "123456";
        string let = "abcd";
        string betlet = "BKPD";

        for (int c = 0; c < 25; c++) {
            skipSecond = false;
            switch (c) {
                case 7: case 12: case 17: skipSecond = true; break;
                default: break;
            }
            if (skipSecond) {
                continue;
            }
            switch (curMaze[c]) {
                case '˂': case '˃': case '˄': case '˅': ends += 1; endLocations.Add(c); break;
                case '├': case '┤': case '┬': case '┴': case '┼': intersections += 1; interLocations.Add(c); break;
                default: break;
            }
        }

        if (ends > 6) {
            int possibleEnds;
            int endIx;
            while (ends > 6) {
                possibleEnds = endLocations.Count();
                endIx = UnityEngine.Random.Range(0, possibleEnds);
                badEnds.Add(endLocations[endIx]);
                endLocations.RemoveAt(endIx);
                ends -= 1;
            }
        }

        for (int d = 0; d < 25; d++) {
            if (endLocations.IndexOf(d) != -1) { //good ending
                interestingMaze += num[endLocations.IndexOf(d)];
                diagramTexts[0] += "O";
                diagramTexts[1] += " ";
            } else if (badEnds.IndexOf(d) != -1) { //bad ending
                interestingMaze += "*";
                diagramTexts[0] += "X";
                diagramTexts[1] += " ";
            } else if (interLocations.IndexOf(d) != -1) { //intersection
                interestingMaze += let[(interLocations.IndexOf(d)+offset)%4];
                diagramTexts[0] += "  ";
                diagramTexts[1] += betlet[(interLocations.IndexOf(d)+offset)%4];
            } else {
                switch (d) {
                    case 7: interestingMaze += "X"; diagramTexts[0] += "III"; diagramTexts[1] += " "; break;
                    case 12: interestingMaze += "Y"; diagramTexts[0] += "III"; diagramTexts[1] += " "; break;
                    case 17: interestingMaze += "Z"; diagramTexts[0] += "III"; diagramTexts[1] += " ";break;
                    default: interestingMaze += "."; diagramTexts[0] += "  "; diagramTexts[1] += " "; break;
                }
            }
            if ((d+1)%5 != 0) {
                diagramTexts[0] += " ";
            } else if (d != 24) {
                diagramTexts[0] += "\n";
                diagramTexts[1] += "\n";
            }
        }
        Diagram[0].text = diagramTexts[0];

        curOri = origins[mazeIx];

        int whichOne = -1;
        for (int z = 0; z < interLocations.Count(); z++) {
            bool[] possibleDoors = {true, true, true, true};
            string dir = "uldr";
            switch (origins[mazeIx][interLocations[z]].ToString()) {
                case "u": possibleDoors[0] = false; break;
                case "l": possibleDoors[1] = false; break;
                case "d": possibleDoors[2] = false; break;
                case "r": possibleDoors[3] = false; break;
                default: Debug.Log("Something has gone wrong! origins[mazeIx][interLocations[z]]"); break;
            }
            switch (curMaze[interLocations[z]]) {
                case '┬': possibleDoors[0] = false; break;
                case '├': possibleDoors[1] = false; break;
                case '┴': possibleDoors[2] = false; break;
                case '┤': possibleDoors[3] = false; break;
                default: /*not a big deal for once...*/ break;
            }
            switch (interestingMaze[interLocations[z]]) {
                case 'a': whichOne = 0; break;
                case 'b': whichOne = 1; break;
                case 'c': whichOne = 2; break;
                case 'd': whichOne = 3; break;
                default: Debug.Log("Something has gone wrong! interestingMaze[interLocations[z]]"); break;
            }
            for (int d = 0; d < 4; d++) {
                if (possibleDoors[d]) {
                    damnDoors[whichOne] += dir[d];
                }
            }
            if (damnDoors[whichOne].Length == 2) {
                damnDoors[whichOne] += "_";
            }
        }
    }

    void GenerateCipher () {
        ciphers = ciphers.Shuffle();
        wordIx = UnityEngine.Random.Range(0, words.Count());
        chosenWord = words[wordIx];
        Debug.LogFormat("[Finite Loop #{0}] Word is {1}", moduleId, chosenWord);
        string a1z26 = " ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string affine = "ZABCDEFGHIJKLMNOPQRSTUVWXY";
        string modern = "QWERTYUIOPASDFGHJKLZXCVBNM";
        for (int l = 0; l < 6; l++) {
            fuckYouGoBack:
            int RNG = UnityEngine.Random.Range(0,26) + 1;
            int ticking = -1;
            switch (ciphers[l]) {
                case "A1Z26": 
                    endLetters[l] = a1z26.IndexOf(chosenWord[l]).ToString();
                break;
                case "CAESAR": 
                    endLetters[l] = a1z26[RNG].ToString();
                    endKeys[l] = ((26 - RNG + a1z26.IndexOf(chosenWord[l]))%26).ToString();
                break;
                case "ATBASH": 
                    endLetters[l] = a1z26[27 - a1z26.IndexOf(chosenWord[l])].ToString();
                break;
                case "AFFINE":
                    endLetters[l] = a1z26[RNG].ToString();
                    while (endKeys[l] == "") {
                        ticking += 1;
                        if ((RNG * (2*ticking+1))%26 == affine.IndexOf(chosenWord[l])) {
                            endKeys[l] = ticking.ToString();
                        } else if (ticking > 26) {
                            goto fuckYouGoBack;
                        }
                    }
                break;
                case "VIGENÈRE":
                    endLetters[l] = a1z26[RNG].ToString();
                    while (endKeys[l] == "") {
                        ticking += 1;
                        if (RNG - ticking == affine.IndexOf(chosenWord[l])) {
                            endKeys[l] = ticking.ToString();
                        } else if (ticking > 26) {
                            goto fuckYouGoBack;
                        }
                    }
                break;
                case "MODERN":
                    endLetters[l] = modern[RNG-1].ToString();
                    ticking = 0;
                    while (endKeys[l] == "") {
                        ticking += 1;
                        if ((RNG-1 + ticking) % 26 == modern.IndexOf(chosenWord[l])) {
                            endKeys[l] = ticking.ToString();
                        } else if (ticking > 26) {
                            goto fuckYouGoBack;
                        }
                    }
                break;
                default: Debug.Log("Something has gone wrong! switch(ciphers[l])"); break;
            }
        }
        Debug.LogFormat("[Finite Loop #{0}] CIPHERS:", moduleId);
        Debug.LogFormat("[Finite Loop #{0}] (1) {1} <{2}> {3}", moduleId, ciphers[0], endKeys[0], endLetters[0]);
        Debug.LogFormat("[Finite Loop #{0}] (2) {1} <{2}> {3}", moduleId, ciphers[1], endKeys[1], endLetters[1]);
        Debug.LogFormat("[Finite Loop #{0}] (3) {1} <{2}> {3}", moduleId, ciphers[2], endKeys[2], endLetters[2]);
        Debug.LogFormat("[Finite Loop #{0}] (4) {1} <{2}> {3}", moduleId, ciphers[3], endKeys[3], endLetters[3]);
        Debug.LogFormat("[Finite Loop #{0}] (5) {1} <{2}> {3}", moduleId, ciphers[4], endKeys[4], endLetters[4]);
        Debug.LogFormat("[Finite Loop #{0}] (6) {1} <{2}> {3}", moduleId, ciphers[5], endKeys[5], endLetters[5]);
    }

    void ShowRoom () {
        areWeAtAnIntersection = false;
        MiddleRoom.SetActive(false);
        DiagramRoom.SetActive(false);
        Button.SetActive(false);
        ButtonGroupObj.SetActive(false);
        KnobObj.SetActive(false);
        Panel.SetActive(false);
        DualityObj.SetActive(false);
        GoodEnd.SetActive(false);
        BadEnd.SetActive(false);
        switch (interestingMaze[curPos]) {
            case 'Y': MiddleRoom.SetActive(true); break;
            case 'X': DiagramRoom.SetActive(true); break;
            case 'Z': Button.SetActive(true); break;
            case 'a': ButtonGroupObj.SetActive(true); areWeAtAnIntersection = true; break;
            case 'b': KnobObj.SetActive(true); areWeAtAnIntersection = true; break;
            case 'c': Panel.SetActive(true); areWeAtAnIntersection = true; break;
            case 'd': DualityObj.SetActive(true); areWeAtAnIntersection = true; break;
            case '1': GoodEnd.SetActive(true); EndText[0].text = "1"; EndText[1].text = endKeys[0]; EndText[2].text = ciphers[0]; EndText[3].text = endLetters[0]; break;
            case '2': GoodEnd.SetActive(true); EndText[0].text = "2"; EndText[1].text = endKeys[1]; EndText[2].text = ciphers[1]; EndText[3].text = endLetters[1]; break;
            case '3': GoodEnd.SetActive(true); EndText[0].text = "3"; EndText[1].text = endKeys[2]; EndText[2].text = ciphers[2]; EndText[3].text = endLetters[2]; break;
            case '4': GoodEnd.SetActive(true); EndText[0].text = "4"; EndText[1].text = endKeys[3]; EndText[2].text = ciphers[3]; EndText[3].text = endLetters[3]; break;
            case '5': GoodEnd.SetActive(true); EndText[0].text = "5"; EndText[1].text = endKeys[4]; EndText[2].text = ciphers[4]; EndText[3].text = endLetters[4]; break;
            case '6': GoodEnd.SetActive(true); EndText[0].text = "6"; EndText[1].text = endKeys[5]; EndText[2].text = ciphers[5]; EndText[3].text = endLetters[5]; break;
            case '*': BadEnd.SetActive(true);
            switch (curMaze[curPos]) {
                case '˂': BadEnd.transform.localRotation = Quaternion.Euler(90f, 270f, 0f); break;
                case '˃': BadEnd.transform.localRotation = Quaternion.Euler(90f, 90f, 0f); break;
                case '˄': BadEnd.transform.localRotation = Quaternion.Euler(90f, 0f, 0f); break;
                case '˅': BadEnd.transform.localRotation = Quaternion.Euler(90f, 180f, 0f); break;
                default: Debug.Log("Something has gone wrong! switch(curMaze[curPos])"); break;
            }
            break;
        }
        for (int a = 0; a < 4; a++) {
            if (areWeAtAnIntersection) {
                ArrowObjs[a].GetComponent<MeshRenderer>().material = ColorMats[4];
            } else {
                ArrowObjs[a].GetComponent<MeshRenderer>().material = ColorMats[3];
            }
        }
        if (directions.IndexOf(curMaze[curPos]) < 8) {
            ArrowObjs[3].GetComponent<MeshRenderer>().material = ColorMats[2];
        }
        if (directions.IndexOf(curMaze[curPos])%8 < 4) {
            ArrowObjs[2].GetComponent<MeshRenderer>().material = ColorMats[2];
        }
        if (directions.IndexOf(curMaze[curPos])%4 < 2) {
            ArrowObjs[1].GetComponent<MeshRenderer>().material = ColorMats[2];
        }
        if (directions.IndexOf(curMaze[curPos])%2 == 0) {
            ArrowObjs[0].GetComponent<MeshRenderer>().material = ColorMats[2];
        }
        if (areWeAtAnIntersection) {
            switch (origins[mazeIx][curPos]) {
                case 'u': ArrowObjs[0].GetComponent<MeshRenderer>().material = ColorMats[6]; break;
                case 'l': ArrowObjs[1].GetComponent<MeshRenderer>().material = ColorMats[6]; break;
                case 'd': ArrowObjs[2].GetComponent<MeshRenderer>().material = ColorMats[6]; break;
                case 'r': ArrowObjs[3].GetComponent<MeshRenderer>().material = ColorMats[6]; break;
                default: Debug.Log("Something has gone wrong! switch(origins[mazeIx][curPos])"); break;
            }
            switch (interestingMaze[curPos]) {
                case 'a': 
                    for (int aa = 0; aa < 3; aa++) {
                        if (LoopDoorFlags[aa]) {
                            switch (damnDoors[0][aa]) {
                                case 'u': ArrowObjs[0].GetComponent<MeshRenderer>().material = ColorMats[5]; break;
                                case 'l': ArrowObjs[1].GetComponent<MeshRenderer>().material = ColorMats[5]; break;
                                case 'd': ArrowObjs[2].GetComponent<MeshRenderer>().material = ColorMats[5]; break;
                                case 'r': ArrowObjs[3].GetComponent<MeshRenderer>().material = ColorMats[5]; break;
                                default: /*it's an _*/ break;
                            }
                        }
                    }
                break;
                case 'b': 
                    for (int bb = 0; bb < 3; bb++) {
                        if (LoopDoorFlags[bb+3]) {
                            switch (damnDoors[1][bb]) {
                                case 'u': ArrowObjs[0].GetComponent<MeshRenderer>().material = ColorMats[5]; break;
                                case 'l': ArrowObjs[1].GetComponent<MeshRenderer>().material = ColorMats[5]; break;
                                case 'd': ArrowObjs[2].GetComponent<MeshRenderer>().material = ColorMats[5]; break;
                                case 'r': ArrowObjs[3].GetComponent<MeshRenderer>().material = ColorMats[5]; break;
                                default: /*it's an _*/ break;
                            }
                        }
                    }
                break;
                case 'c': 
                    for (int cc = 0; cc < 3; cc++) {
                        if (LoopDoorFlags[cc+6]) {
                            switch (damnDoors[2][cc]) {
                                case 'u': ArrowObjs[0].GetComponent<MeshRenderer>().material = ColorMats[5]; break;
                                case 'l': ArrowObjs[1].GetComponent<MeshRenderer>().material = ColorMats[5]; break;
                                case 'd': ArrowObjs[2].GetComponent<MeshRenderer>().material = ColorMats[5]; break;
                                case 'r': ArrowObjs[3].GetComponent<MeshRenderer>().material = ColorMats[5]; break;
                                default: /*it's an _*/ break;
                            }
                        }
                    }
                break;
                case 'd': 
                    for (int dd = 0; dd < 3; dd++) {
                        if (LoopDoorFlags[dd+9]) {
                            switch (damnDoors[3][dd]) {
                                case 'u': ArrowObjs[0].GetComponent<MeshRenderer>().material = ColorMats[5]; break;
                                case 'l': ArrowObjs[1].GetComponent<MeshRenderer>().material = ColorMats[5]; break;
                                case 'd': ArrowObjs[2].GetComponent<MeshRenderer>().material = ColorMats[5]; break;
                                case 'r': ArrowObjs[3].GetComponent<MeshRenderer>().material = ColorMats[5]; break;
                                default: /*it's an _*/ break;
                            }
                        }
                    }
                break;
                default: Debug.Log("Something has gone wrong! interestingMaze[curPos]"); break;
            }
        }
        Coord.text = coordSet[curPos];
    }

    void StartLoop() {
        if (moduleSolved) {
            return;
        }
        InLoop = true;
        LoopNumber.text = ResetCount.ToString();
        Loop = StartCoroutine(LoopCoro());
    }

    IEnumerator LoopCoro() {
        while (true) {
            LoopTime += Time.deltaTime;
            if (LoopPointer >= LoopPoints.Count()) {
                //lamo
            } else if (LoopTime > LoopPoints[LoopPointer]) {
                if (LoopRuns[LoopPointer] != ResetCount) { //Fix doubling bug the mod had to deal with for over a year. I wish that was a joke.
                    PressAnything(false, LoopActions[LoopPointer]);
                }
                LoopPointer += 1;
            }
            yield return null;
        }
    }

    void PressAnything (bool x, string s) {
        if (!InLoop || moduleSolved) {
            return;
        }
        if (x && !(LoopPointer > LoopPoints.Count())) { //!(LoopPointer > LoopPoints.Count()) fixes an IndexOutOfRangeException
            LoopPoints.Insert(LoopPointer, LoopTime);
            LoopActions.Insert(LoopPointer, s);
            LoopRuns.Insert(LoopPointer, ResetCount);
            Debug.LogFormat("[Finite Loop #{0}] {1} at {2} during reset {3}", moduleId, s, LoopTime, ResetCount);
        }
        switch (s) {
            case "DOT 1 HOLD": LoopObjFlags[0] = 1; MiddleDotObjs[0].GetComponent<MeshRenderer>().material = ColorMats[1]; break;
            case "DOT 1 RELEASE": LoopObjFlags[0] = 0; MiddleDotObjs[0].GetComponent<MeshRenderer>().material = ColorMats[0]; break;
            case "DOT 2 HOLD": LoopObjFlags[1] = 1; MiddleDotObjs[1].GetComponent<MeshRenderer>().material = ColorMats[1]; break;
            case "DOT 2 RELEASE": LoopObjFlags[1] = 0; MiddleDotObjs[1].GetComponent<MeshRenderer>().material = ColorMats[0]; break;
            case "DOT 3 HOLD": LoopObjFlags[2] = 1; MiddleDotObjs[2].GetComponent<MeshRenderer>().material = ColorMats[1]; break;
            case "DOT 3 RELEASE": LoopObjFlags[2] = 0; MiddleDotObjs[2].GetComponent<MeshRenderer>().material = ColorMats[0]; break;
            case "DOT 4 HOLD": LoopObjFlags[3] = 1; MiddleDotObjs[3].GetComponent<MeshRenderer>().material = ColorMats[1]; break;
            case "DOT 4 RELEASE": LoopObjFlags[3] = 0; MiddleDotObjs[3].GetComponent<MeshRenderer>().material = ColorMats[0]; break;
            case "DOT 5 HOLD": LoopObjFlags[4] = 1; MiddleDotObjs[4].GetComponent<MeshRenderer>().material = ColorMats[1]; break;
            case "DOT 5 RELEASE": LoopObjFlags[4] = 0; MiddleDotObjs[4].GetComponent<MeshRenderer>().material = ColorMats[0]; break;
            case "DOT 6 HOLD": LoopObjFlags[5] = 1; MiddleDotObjs[5].GetComponent<MeshRenderer>().material = ColorMats[1]; break;
            case "DOT 6 RELEASE": LoopObjFlags[5] = 0; MiddleDotObjs[5].GetComponent<MeshRenderer>().material = ColorMats[0]; break;
            case "SUBMIT PRESS": if (x) { GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform); } SubmitPress(); break;
            case "SWITCH TOGGLE": if (x) { GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BriefcaseOpen, transform); } LoopObjFlags[6] += 1; SwitchFlip(LoopObjFlags[6]%2); break;
            case "HARD RESET PRESS": if (x) { GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, transform); } LoopObjFlags[7] += 1; HardResetPress(LoopObjFlags[7]); break;
            case "GROUP BUTTON 1 HOLD": LoopObjFlags[8] = 1; ButtonGroupObjs[0].GetComponent<MeshRenderer>().material = ColorMats[1]; LoopDoorFlags[0] = true; break;
            case "GROUP BUTTON 1 RELEASE": LoopObjFlags[8] = 0; ButtonGroupObjs[0].GetComponent<MeshRenderer>().material = ColorMats[0]; LoopDoorFlags[0] = false; break;
            case "GROUP BUTTON 2 HOLD": LoopObjFlags[9] = 1; ButtonGroupObjs[1].GetComponent<MeshRenderer>().material = ColorMats[1]; LoopDoorFlags[1] = true; break;
            case "GROUP BUTTON 2 RELEASE": LoopObjFlags[9] = 0; ButtonGroupObjs[1].GetComponent<MeshRenderer>().material = ColorMats[0]; LoopDoorFlags[1] = false; break;
            case "GROUP BUTTON 3 HOLD": LoopObjFlags[10] = 1; ButtonGroupObjs[2].GetComponent<MeshRenderer>().material = ColorMats[1]; LoopDoorFlags[2] = true; break;
            case "GROUP BUTTON 3 RELEASE": LoopObjFlags[10] = 0; ButtonGroupObjs[2].GetComponent<MeshRenderer>().material = ColorMats[0]; LoopDoorFlags[2] = false; break;
            case "KNOB TURN": if (x) { GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BombDrop, transform); } LoopObjFlags[11] = (LoopObjFlags[11]+1)%3; TurnKnob(LoopObjFlags[11]); break;
            case "PANEL BUTTON 1 HOLD": LoopObjFlags[12] = 1; PanelButtonObjs[0].GetComponent<MeshRenderer>().material = ColorMats[1]; LoopDoorFlags[9] = true; break;
            case "PANEL BUTTON 1 RELEASE": LoopObjFlags[12] = 0; PanelButtonObjs[0].GetComponent<MeshRenderer>().material = ColorMats[0]; LoopDoorFlags[9] = false; break;
            case "PANEL BUTTON 2 HOLD": LoopObjFlags[13] = 1; PanelButtonObjs[1].GetComponent<MeshRenderer>().material = ColorMats[1]; LoopDoorFlags[10] = true; break;
            case "PANEL BUTTON 2 RELEASE": LoopObjFlags[13] = 0; PanelButtonObjs[1].GetComponent<MeshRenderer>().material = ColorMats[0]; LoopDoorFlags[10] = false; break;
            case "PANEL BUTTON 3 HOLD": LoopObjFlags[14] = 1; PanelButtonObjs[2].GetComponent<MeshRenderer>().material = ColorMats[1]; LoopDoorFlags[11] = true; break;
            case "PANEL BUTTON 3 RELEASE": LoopObjFlags[14] = 0; PanelButtonObjs[2].GetComponent<MeshRenderer>().material = ColorMats[0]; LoopDoorFlags[11] = false; break;
            default: Debug.Log("Something has gone wrong! I don't know what " + s + "is!"); break;
        }
        LoopObjFlags[15] = LoopObjFlags[12] + LoopObjFlags[13] + LoopObjFlags[14];
        switch (LoopObjFlags[15]) {
            case 0: LoopDoorFlags[6] = false; LoopDoorFlags[7] = false; LoopDoorFlags[8] = false; break;
            case 1: LoopDoorFlags[6] = true; LoopDoorFlags[7] = false; LoopDoorFlags[8] = false; break;
            case 2: LoopDoorFlags[6] = true; LoopDoorFlags[7] = true; LoopDoorFlags[8] = false; break;
            case 3: LoopDoorFlags[6] = true; LoopDoorFlags[7] = true; LoopDoorFlags[8] = true; break;
            default: Debug.Log("Something has gone wrong! LoopObjFlags[15]"); break;
        }
        PanelDisplay.text = LoopObjFlags[15].ToString();
        ShowRoom();
    }

    void SoftReset() {
        InLoop = false;
        StopCoroutine(Loop);
        ResetText.text = "HARD\nRESET";
        LoopNumber.text = " ";
        LoopPointer = 0;
        if (LoopTime != 0f) {
            Debug.LogFormat("[Finite Loop #{0}] Reset {1} lasted for {2}", moduleId, ResetCount, LoopTime);
        }
        LoopTime = 0f;
        ResetCount += 1;
        curPos = 12;
        LoopAnswer = "";
        MiddleWord.text = "";
        SwitchObject.transform.localPosition = new Vector3( -0.057f, 0.029f, 0.032f);
        SwitchObject.transform.localRotation = Quaternion.Euler(-30f, 0f, 0f);
        MazeObj.sprite = MazeSprites[mazeIx+1];
        Diagram[0].text = diagramTexts[0];
        Diagram[1].text = " ";
        PanelDisplay.text = "0";
        KnobObj.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        for (int k = 0; k < LoopObjFlags.Count(); k++) {
            LoopObjFlags[k] = 0;
            if (k < 12) {
                if (k != 3) {
                    LoopDoorFlags[k] = false;
                } else {
                    LoopDoorFlags[k] = true;
                }
            }
            if (k < 3) {
                MiddleDotObjs[k].GetComponent<MeshRenderer>().material = ColorMats[0];
                PanelButtonObjs[k].GetComponent<MeshRenderer>().material = ColorMats[0];
            }
            if (k < 6) {
                MiddleDotObjs[k].GetComponent<MeshRenderer>().material = ColorMats[0];
            }
        }
        CursedDuality(ResetCount%3);
        ShowRoom();
    }

    void SubmitPress() {
        string nuts = "123456";
        string gex = "";
        for (int q = 0; q < 6; q++) {
            if (LoopObjFlags[q] == 1) {
                gex += nuts[q];
            }
        }
        switch (gex) {
            case "1": LoopAnswer += "A"; break;
            case "12": LoopAnswer += "B"; break;
            case "14": LoopAnswer += "C"; break;
            case "145": LoopAnswer += "D"; break;
            case "15": LoopAnswer += "E"; break;
            case "124": LoopAnswer += "F"; break;
            case "1245": LoopAnswer += "G"; break;
            case "125": LoopAnswer += "H"; break;
            case "24": LoopAnswer += "I"; break;
            case "245": LoopAnswer += "J"; break;
            case "13": LoopAnswer += "K"; break;
            case "123": LoopAnswer += "L"; break;
            case "134": LoopAnswer += "M"; break;
            case "1345": LoopAnswer += "N"; break;
            case "135": LoopAnswer += "O"; break;
            case "1234": LoopAnswer += "P"; break;
            case "12345": LoopAnswer += "Q"; break;
            case "1235": LoopAnswer += "R"; break;
            case "234": LoopAnswer += "S"; break;
            case "2345": LoopAnswer += "T"; break;
            case "136": LoopAnswer += "U"; break;
            case "1236": LoopAnswer += "V"; break;
            case "2456": LoopAnswer += "W"; break;
            case "1346": LoopAnswer += "X"; break;
            case "13456": LoopAnswer += "Y"; break;
            case "1356": LoopAnswer += "Z"; break;
            default: LoopAnswer += "?"; break;
        }
        MiddleWord.text = LoopAnswer;
        if (LoopAnswer.Length == 6) {
            if (LoopAnswer == chosenWord) {
                Audio.PlaySoundAtTransform("seed_complete_main", transform);
                moduleSolved = true;
                GetComponent<KMBombModule>().HandlePass();
                Debug.LogFormat("[Finite Loop #{0}] {1} submitted, that is correct! Module solved!", moduleId, LoopAnswer);
            } else {
                GetComponent<KMBombModule>().HandleStrike();
                Debug.LogFormat("[Finite Loop #{0}] {1} submitted, that is incorrect. Strike! Hard resetting...", moduleId, LoopAnswer);
                HardReset();
            }
        }
    }

    void HardResetPress(int r) {
        if (r > 1) {
            HardReset();
        } else {
            ResetText.text = "ARE YOU\nSURE?";
        }
    }

    void HardReset() {
        Debug.LogFormat("[Finite Loop #{0}] HARD RESET!", moduleId);
        SoftReset();
        ResetCount = 0;
        for (int p = 0; p < 6; p++) { //these are here so that it goes back to the initial state
            MiddleDotObjs[p].GetComponent<MeshRenderer>().material = ColorMats[0];
            if (p < 3) {
                ButtonGroupObjs[p].GetComponent<MeshRenderer>().material = ColorMats[0];  
                PanelButtonObjs[p].GetComponent<MeshRenderer>().material = ColorMats[0];
            }
        }
        LoopPoints.Clear();
        LoopActions.Clear();
        StartLoop();
    }

    void SwitchFlip(int d) {
        if (d == 0) { //TOP
            SwitchObject.transform.localPosition = new Vector3( -0.057f, 0.029f, 0.032f);
            SwitchObject.transform.localRotation = Quaternion.Euler(-30f, 0f, 0f);
            MazeObj.sprite = MazeSprites[mazeIx+1];
            Diagram[0].text = diagramTexts[0];
            Diagram[1].text = " ";
        } else { //BOTTOM
            SwitchObject.transform.localPosition = new Vector3( -0.057f, 0.029f, -0.004f);
            SwitchObject.transform.localRotation = Quaternion.Euler(-30f, -180f, 0f);
            MazeObj.sprite = MazeSprites[0];
            Diagram[0].text = " ";
            Diagram[1].text = diagramTexts[1];
        }
    }

    void TurnKnob(int y) {
        switch (y) {
            case 0: KnobObj.transform.localRotation = Quaternion.Euler(0f, 180f, 0f); LoopDoorFlags[3] = true; LoopDoorFlags[4] = false; LoopDoorFlags[5] = false; break;
            case 1: KnobObj.transform.localRotation = Quaternion.Euler(0f, 300f, 0f); LoopDoorFlags[3] = false; LoopDoorFlags[4] = true; LoopDoorFlags[5] = false; break;
            case 2: KnobObj.transform.localRotation = Quaternion.Euler(0f, 60f, 0f); LoopDoorFlags[3] = false; LoopDoorFlags[4] = false; LoopDoorFlags[5] = true; break;
            default: Debug.Log("Something has gone wrong! switch(y)"); break;
        }
    }

    void CursedDuality(int r) {
        switch (r) {
            case 0: DualityObj.transform.localRotation = Quaternion.Euler(90f, 0f, 0f); LoopDoorFlags[9] = true; break;
            case 1: DualityObj.transform.localRotation = Quaternion.Euler(90f, 120f, 0f); LoopDoorFlags[10] = true; break;
            case 2: DualityObj.transform.localRotation = Quaternion.Euler(90f, 240f, 0f); LoopDoorFlags[11] = true; break;
            default: Debug.Log("Something has gone wrong! switch(r)"); break;
        }
    }
    
    void ArrowPress(KMSelectable Arrow) {
        char pressed = ' ';
        bool good = false;
        if (!InLoop) {
            return;
        }
        if (Arrow == Arrows[0]) { //U
            pressed = 'u';
        } else if (Arrow == Arrows[1]) { //L
            pressed = 'l';
        } else if (Arrow == Arrows[2]) { //D
            pressed = 'd';
        } else if (Arrow == Arrows[3]) { //R
            pressed = 'r';
        }
        
        if (curOri[curPos] == pressed) {
            good = true;
        }
        if (areWeAtAnIntersection) {
            switch (interestingMaze[curPos]) {
                case 'a':  
                    if (damnDoors[0].IndexOf(pressed) != -1) {
                        if (LoopDoorFlags[damnDoors[0].IndexOf(pressed)]) {
                            good = true;
                        }
                    }
                break;
                case 'b':  
                    if (LoopDoorFlags[damnDoors[1].IndexOf(pressed)+3]) {
                        good = true;
                    }
                break;
                case 'c': 
                    if (LoopDoorFlags[damnDoors[2].IndexOf(pressed)+6]) {
                        good = true;
                    }
                break;
                case 'd':  
                    if (LoopDoorFlags[damnDoors[3].IndexOf(pressed)+9]) {
                        good = true;
                    }
                break;
                default: Debug.Log("Something has gone wrong! switch (interestingMaze[curPos])"); break;
            }
        } else {
            good = true;
        }
            
        if (good) {
            switch (pressed) {
                case 'u': 
                    if (directions.IndexOf(curMaze[curPos])%2 == 1) {
                        curPos -= 5; ShowRoom();
                    }
                break;
                case 'l': 
                    if (directions.IndexOf(curMaze[curPos])%4 > 1) {
                        curPos -= 1; ShowRoom();
                    }
                break;
                case 'd': 
                    if (directions.IndexOf(curMaze[curPos])%8 > 3) {
                        curPos += 5; ShowRoom();
                    }
                break;
                case 'r': 
                    if (directions.IndexOf(curMaze[curPos]) > 7) {
                        curPos += 1; ShowRoom();
                    }
                break;
                default: Debug.Log("Something has gone wrong! switch (pressed)"); break;
            }
        }
    }

    void TestPress() {
        InLoop = !InLoop;
        if (InLoop) {
            StartLoop();
        } else {
            SoftReset();
        }
    }

    //twitch plays
    bool TwitchShouldCancelCommand;
    KMSelectable heldObj = null;
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} move/m <left/l/right/r/up/u/down/d> [Move through rooms] | !{0} hold/h/tap/t <#> [Holds/taps object '#' in reading order (unless you are at C3, in which case it is Braille order)] | !{0} release/r [Releases the held object] | !{0} wait/w <#> [Waits for '#' seconds] | Each individual command will select the module, execute the specified action, and then deselect | To perform multiple actions in one command, separate each with commas or semicolons";
    #pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] actions = command.Split(new char[] { ',', ';' });
        for (int i = 0; i < actions.Length; i++)
        {
            bool pass = false;
            string[] actionSplit = actions[i].Split(' ');
            if (actionSplit.Length == 2)
            {
                if (actionSplit[0].ToLower().EqualsAny("move", "m") && actionSplit[1].ToLower().EqualsAny("left", "l", "right", "r", "up", "u", "down", "d"))
                    pass = true;
                if (actionSplit[0].ToLower().EqualsAny("hold", "h", "tap", "t") && actionSplit[1].ToLower().EqualsAny("1", "2", "3", "4", "5", "6", "7"))
                    pass = true;
                float temp;
                if (actionSplit[0].ToLower().EqualsAny("wait", "w") && float.TryParse(actionSplit[1], out temp) && temp > 0f)
                    pass = true;
            }
            else if (actionSplit.Length == 1)
            {
                if (actionSplit[0].ToLower().EqualsAny("release", "r"))
                    pass = true;
            }
            if (!pass)
                yield break;
        }
        yield return null;
        for (int i = 0; i < actions.Length; i++)
        {
            string[] actionSplit = actions[i].Split(' ');
            switch (actionSplit[0].ToLower())
            {
                case "move":
                case "m":
                    if (heldObj == null)
                        Arrows["uldr".IndexOf(actionSplit[1].ToLower()[0])].OnInteract();
                    break;
                case "release":
                case "r":
                    if (heldObj != null)
                    {
                        heldObj.OnInteractEnded();
                        heldObj = null;
                    }
                    break;
                case "wait":
                case "w":
                    float t = 0f;
                    while (t < float.Parse(actionSplit[1]))
                    {
                        t += Time.deltaTime;
                        yield return null;
                        if (TwitchShouldCancelCommand)
                            goto exit;
                    }
                    break;
                default:
                    List<KMSelectable> roomSelectables = new List<KMSelectable>();
                    switch (interestingMaze[curPos])
                    {
                        case 'Y': roomSelectables.AddRange(MiddleDots); roomSelectables.Add(MiddleSubmit); break;
                        case 'X': roomSelectables.Add(DiagramSwitch); break;
                        case 'Z': roomSelectables.Add(HardResetButton); break;
                        case 'a': roomSelectables.AddRange(ButtonGroup); break;
                        case 'b': roomSelectables.Add(Knob); break;
                        case 'c': roomSelectables.AddRange(PanelButtons); break;
                    }
                    int index = int.Parse(actionSplit[1]);
                    if (index <= roomSelectables.Count)
                    {
                        if (actionSplit[0].ToLower().EqualsAny("tap", "t") && heldObj == null)
                        {
                            roomSelectables[index - 1].OnInteract();
                            if (roomSelectables[index - 1].OnInteractEnded != null)
                                roomSelectables[index - 1].OnInteractEnded();
                        }
                        else if (heldObj == null && roomSelectables[index - 1].OnInteractEnded != null)
                        {
                            roomSelectables[index - 1].OnInteract();
                            heldObj = roomSelectables[index - 1];
                        }
                    }
                    break;
            }
            yield return null;
        }
        exit:
        if (heldObj != null)
        {
            heldObj.OnInteractEnded();
            heldObj = null;
        }
        if (TwitchShouldCancelCommand)
            yield return "cancelled";
    }

    private string[] FREAKINBRAILLE = { "1", "12", "14", "145", "15", "124", "1245", "125", "24", "245", "13", "123", "134", "1345", "135", "1234", "12345", "1235", "234", "2345", "136", "1236", "2456", "1346", "13456", "1356" };
    IEnumerator TwitchHandleForcedSolve()
    {
        if (InLoop)
        {
            StopAllCoroutines();
            moduleSolved = true;
            GetComponent<KMBombModule>().HandlePass();
            yield break;
        }
        if (LoopActions.Contains("DOT 1 HOLD") || LoopActions.Contains("DOT 2 HOLD") || LoopActions.Contains("DOT 3 HOLD") || LoopActions.Contains("DOT 4 HOLD") || LoopActions.Contains("DOT 5 HOLD") || LoopActions.Contains("DOT 6 HOLD"))
        {
            ModuleSelectable.OnFocus();
            Arrows[2].OnInteract();
            yield return new WaitForSeconds(.1f);
            HardResetButton.OnInteract();
            yield return new WaitForSeconds(.1f);
            HardResetButton.OnInteract();
            yield return new WaitForSeconds(.1f);
            ModuleSelectable.OnDefocus();
        }
        for (int i = 0; i < 6; i++)
        {
            int funny = -1;
            for (int j = 0; j < 6; j++)
            {
                if (FREAKINBRAILLE[chosenWord[j] - 'A'].Contains((i + 1).ToString()))
                    funny = j;
            }
            if (funny != -1)
            {
                ModuleSelectable.OnFocus();
                for (int j = 0; j < funny + 1; j++)
                {
                    bool held = false;
                    if (FREAKINBRAILLE[chosenWord[j] - 'A'].Contains((i + 1).ToString()))
                    {
                        MiddleDots[i].OnInteract();
                        held = true;
                    }
                    yield return new WaitForSeconds(1f);
                    if (held)
                        MiddleDots[i].OnInteractEnded();
                    yield return new WaitForSeconds(1f);
                }
                ModuleSelectable.OnDefocus();
            }
        }
        ModuleSelectable.OnFocus();
        yield return new WaitForSeconds(.5f);
        for (int i = 0; i < 6; i++)
        {
            MiddleSubmit.OnInteract();
            if (i != 5)
                yield return new WaitForSeconds(2f);
            else
                yield return new WaitForSeconds(.5f);
        }
        ModuleSelectable.OnDefocus();
    }
}