Environment = luanet.import_type 'System.Environment'

function ClickAt(tab)
    bot:log("ClickAt " .. tab.x .. "," .. tab.y);
    bot:ClickAt(tab.x,tab.y);
end;

function onDungeonClear(tab)
    bot:ActiveBlueStackWindow();
    bot:ClickAt(tab.x,tab.y);
    bot:AddTask("bot:log('onDungeonClear')",3000);
end;

function onQuestItemDone(tab)
    
end;

function ProcessTask()
    local now = Environment.TickCount;
    --bot:log();
    bot.lstTask:RefreshItems();
    for i=0, bot.lstTask.Items.Count -1 do
        local item = bot.lstTask.Items[i];
        if now > item.execTime then
            local f = loadstring(item.script);
            if f() ~= true then
                bot.lstTask.Items:RemoveAt(i);
            end;
            return true
        end;
    end;
    return false;
end;

function Auto()
    if bot.chkAutoClick.Checked == true then
        -- bot:log(classifyResult:ToString());
        local screenName = classifyResult.label;
        local screens = stateTable[currentState];
        if bot.screenClassifiers:ContainsKey(screenName) == true then
            local subScreenClassifier = bot.screenClassifiers[screenName];
            local subResult = subScreenClassifier:Classify(currentBitMap, 0.9);
            if subResult ~= nil then
                screenName = screenName .. "/" .. subResult.label;
            end;
        end;
        bot:log("State " .. currentState .. ' ' .. screenName);
        local screen = GetStateTable(currentState,screenName);
        if screen ~= nil then
            screen:cmd();
        end;
    end;
    bot.txtDebug:ScrollToCaret();
end;

function GetStateTable(state,screen)
    local screens = stateTable[state];
    for i=1,table.getn(screens) do
        if(screens[i].screen == screen) then
            return screens[i];
        end;
    end;
    return nil;
end;

stateTable = {};
stateTable["Auto"] = {
    {screen="QuestSkip",cmd=ClickAt,x=1180,y=510},
    {screen="QuestComplete",cmd=ClickAt,x=634,y=615},
    {screen="QuestDo",cmd=ClickAt,x=763,y=613},
    {screen="QuestAccept",cmd=ClickAt,x=763,y=613},
    {screen="Move",cmd=ClickAt,x=511,y=519},
    {screen="Dead",cmd=ClickAt,x=1199,y=252},
    {screen="PartyRequest",cmd=ClickAt,x=900,y=56},
    {screen="HarvestExit",cmd=ClickAt,x=760,y=482},
    {screen="QuestSelectB2",cmd=ClickAt,x=564,y=222},
    {screen="QuestSelectB3",cmd=ClickAt,x=700,y=222},
    {screen="QuestSelected",cmd=ClickAt,x=850,y=600},
    {screen="QuestConfirm",cmd=ClickAt,x=760,y=500},
    {screen="ClearDungeon",cmd=onDungeonClear,x=956,y=635},
    {screen="DungeonEXP/NoMore",cmd=ClickAt,x=1240,y=40},   -- Exit
    {screen="DungeonGold/CanDo",cmd=ClickAt,x=1080,y=640},   
    


    
    {screen="WeeklyQuest/NoQuest",cmd=ClickAt,x=1238,y=45},
    {screen="WeeklyQuest/DoQuest",cmd=ClickAt,x=940,y=420},
    {screen="WeeklyQuest/QuestSuccess",cmd=ClickAt,x=937,y=423},
    {screen="WeeklyQuest/MoveNow",cmd=ClickAt,x=940,y=420},
    {screen="WeeklyQuest/NoQuest",cmd=ClickAt,x=1242,y=40},
    
    {screen="Main/Success",cmd=ClickAt,x=260,y=330},
    {screen="Main/Harvest",cmd=ClickAt,x=689,y=262},
    {screen="Main/HarvestDone",cmd=ClickAt,x=863,y=84},
    {screen="Main/PartyAutoNoSkill",cmd=ClickAt,x=874,y=683},
    {screen="Main/Quest2",cmd=ClickAt,x=52,y=312},
};

stateTable["AutoQuest"] = {
    {screen="QuestSkip",cmd=ClickAt,x=1180,y=510},
    {screen="QuestComplete",cmd=ClickAt,x=634,y=615},
    {screen="QuestDo",cmd=ClickAt,x=763,y=613},
    {screen="QuestAccept",cmd=ClickAt,x=763,y=613},
    {screen="Move",cmd=ClickAt,x=511,y=519},
    {screen="Dead",cmd=ClickAt,x=1199,y=252},
    {screen="PartyRequest",cmd=ClickAt,x=900,y=56},
    {screen="HarvestExit",cmd=ClickAt,x=760,y=482},
    {screen="QuestSelectB2",cmd=ClickAt,x=564,y=222},
    {screen="QuestSelectB3",cmd=ClickAt,x=700,y=222},
    {screen="QuestSelected",cmd=ClickAt,x=850,y=600},
    {screen="QuestConfirm",cmd=ClickAt,x=760,y=500},
    {screen="ClearDungeon",cmd=onDungeonClear,x=956,y=635},
    {screen="DungeonEXP/NoMore",cmd=ClickAt,x=1240,y=40},   -- Exit
    {screen="DungeonGold/CanDo",cmd=ClickAt,x=1080,y=640},   
    


    
    {screen="WeeklyQuest/NoQuest",cmd=ClickAt,x=1238,y=45},
    {screen="WeeklyQuest/DoQuest",cmd=ClickAt,x=940,y=420},
    {screen="WeeklyQuest/QuestSuccess",cmd=ClickAt,x=937,y=423},
    {screen="WeeklyQuest/MoveNow",cmd=ClickAt,x=940,y=420},
    {screen="WeeklyQuest/NoQuest",cmd=ClickAt,x=1242,y=40},
    
    {screen="Main/Success",cmd=ClickAt,x=260,y=330},
    {screen="Main/Harvest",cmd=ClickAt,x=689,y=262},
    {screen="Main/HarvestDone",cmd=ClickAt,x=863,y=84},
    {screen="Main/PartyAutoNoSkill",cmd=ClickAt,x=874,y=683},
    {screen="Main/Quest2",cmd=ClickAt,x=52,y=312},
    
    {screen="Bag",cmd=ClickAt,x=1145,y=130},
    {screen="Bag/item3",cmd=ClickAt,x=833,y=471},
    {screen="Bag/questItem",cmd=ClickAt,x=705,y=397},
};
currentState = "Auto";



bot:log("Main.lua loaded");