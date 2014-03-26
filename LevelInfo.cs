using System;

public class LevelInfo
{
    public string desc;
    public int enemyNumber;
    private static bool init;
    public static LevelInfo[] levels;
    public string name;
    public RespawnMode respawnMode;
    public GAMEMODE type;

    public static LevelInfo getInfo(string name)
    {
        initData();
        foreach (LevelInfo info in levels)
        {
            if (info.name == name)
            {
                return info;
            }
        }
        return null;
    }

    private static void initData()
    {
        if (!init)
        {
            init = true;
            levels = new LevelInfo[] { new LevelInfo(), new LevelInfo(), new LevelInfo(), new LevelInfo(), new LevelInfo(), new LevelInfo(), new LevelInfo(), new LevelInfo(), new LevelInfo(), new LevelInfo(), new LevelInfo(), new LevelInfo() };
            levels[0].name = "The City I";
            levels[0].desc = "kill 32 titans with your friends.(No RESPAWN/NO SUPPLY)";
            levels[0].enemyNumber = 0x20;
            levels[0].type = GAMEMODE.KILL_TITAN;
            levels[0].respawnMode = RespawnMode.NEVER;
            levels[1].name = "The City II";
            levels[1].desc = "kill 32 titans with your friends.(RESPAWN AFTER 10 SECONDS/SUPPLY)";
            levels[1].enemyNumber = 0x20;
            levels[1].type = GAMEMODE.KILL_TITAN;
            levels[1].respawnMode = RespawnMode.DEATHMATCH;
            levels[2].name = "Cage Fighting";
            levels[2].desc = "2 players in different cages. when you kill a titan,  one or more titan will spawn to your opponent's cage.";
            levels[2].enemyNumber = 1;
            levels[2].type = GAMEMODE.CAGE_FIGHT;
            levels[2].respawnMode = RespawnMode.NEVER;
            levels[3].name = "The Forest";
            levels[3].desc = "The Forest Of Giant Trees.Titans will respawn from nowhere.";
            levels[3].enemyNumber = 15;
            levels[3].type = GAMEMODE.ENDLESS_TITAN;
            levels[3].respawnMode = RespawnMode.DEATHMATCH;
            levels[4].name = "The Forest II";
            levels[4].desc = "Survive for 20 waves.";
            levels[4].enemyNumber = 3;
            levels[4].type = GAMEMODE.SURVIVE_MODE;
            levels[4].respawnMode = RespawnMode.NEVER;
            levels[5].name = "The Forest III";
            levels[5].desc = "Survive for 20 waves.player will respawn in every new wave";
            levels[5].enemyNumber = 3;
            levels[5].type = GAMEMODE.SURVIVE_MODE;
            levels[5].respawnMode = RespawnMode.NEWROUND;
            levels[6].name = "Annie";
            levels[6].desc = "Nape Armor/ Ankle Armor:\nNormal:1000/50\nHard:2500/100\nAbnormal:4000/200\nYou only have 1 life.Don't do this alone.";
            levels[6].enemyNumber = 15;
            levels[6].type = GAMEMODE.KILL_TITAN;
            levels[6].respawnMode = RespawnMode.NEVER;
            levels[7].name = "Annie II";
            levels[7].desc = "Nape Armor/ Ankle Armor:\nNormal:1000/50\nHard:3000/200\nAbnormal:6000/1000\n(RESPAWN AFTER 10 SECONDS)";
            levels[7].enemyNumber = 15;
            levels[7].type = GAMEMODE.KILL_TITAN;
            levels[7].respawnMode = RespawnMode.DEATHMATCH;
            levels[8].name = "Colossal Titan";
            levels[8].desc = "Defeat the Colossal Titan.\nPrevent the abnormal titan from running to the north gate.\n Nape Armor:\n Normal:2000\nHard:3500\nAbnormal:5000\n";
            levels[8].enemyNumber = 2;
            levels[8].type = GAMEMODE.BOSS_FIGHT_CT;
            levels[8].respawnMode = RespawnMode.NEVER;
            levels[9].name = "Colossal Titan II";
            levels[9].desc = "Defeat the Colossal Titan.\nPrevent the abnormal titan from running to the north gate.\n Nape Armor:\n Normal:5000\nHard:8000\nAbnormal:12000\n(RESPAWN AFTER 10 SECONDS)";
            levels[9].enemyNumber = 2;
            levels[9].type = GAMEMODE.BOSS_FIGHT_CT;
            levels[9].respawnMode = RespawnMode.DEATHMATCH;
            levels[10].name = "Trost";
            levels[10].desc = "Escort Titan Eren";
            levels[10].enemyNumber = 2;
            levels[10].type = GAMEMODE.TROST;
            levels[10].respawnMode = RespawnMode.NEVER;
            levels[11].name = "Trost II";
            levels[11].desc = "Escort Titan Eren(RESPAWN AFTER 10 SECONDS)";
            levels[11].enemyNumber = 2;
            levels[11].type = GAMEMODE.TROST;
            levels[11].respawnMode = RespawnMode.DEATHMATCH;
        }
    }
}

