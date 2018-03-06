function ClickAt(tab)
    bot:log("ClickAt " .. tab.x .. "," .. tab.y);
    bot:ClickAt(tab.x,tab.y);
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

--[[
{"QuestSkip":"1180, 510","QuestComplete":"634, 615","QuestDo":"763, 613","QuestAccept":"763, 613","Move":"511,519", "Dead":"1199,252", "PartyRequest":"900,56"
, "WeeklyQuest/NoQuest":"1238,45","WeeklyQuest/DoQuest":"940,420","WeeklyQuest/QuestSuccess":"937,423","WeeklyQuest/MoveNow":"940,420","WeeklyQuest/NoQuest":"1242,40"
, "Main/Success":"260,330","Main/Harvest":"689,262","Main/HarvestDone":"863,84","HarvestExit":"760,482","Main/PartyManual":"874,683","Main/PartyAutoNoSkill":"874,683"
, "Main/Quest2":"52,312","QuestSelectB2":"564,222","QuestSelectB3":"700,222","QuestSelected":"850,600","QuestConfirm":"760,500"
}
--]]    

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
currentState = "Auto";



bot:log("Main.lua loaded");