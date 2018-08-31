--NOX adb connect 127.0.0.1:62001
--adb forward tcp:52736 tcp:52736 

Environment = luanet.import_type 'System.Environment'
function string.starts(String,Start)
   return string.sub(String,1,string.len(Start))==Start
end

function string.ends(String,End)
   return End=='' or string.sub(String,-string.len(End))==End
end

function ArrayToString(arr)
    local ret = "";
    for i=0,arr.Length -1 do
        ret = ret .. " " .. arr[i];
    end;
    return ret;
end;

function ClickAt(tab)
    if tab.clickPoint ~= nil then
        Click(tab.clickPoint);
        return;
    end;
    bot:log("ClickAt " .. tab.x .. "," .. tab.y);
    bot:ClickAt(tab.x,tab.y);
end;

function onDungeonClear(tab)
    bot:ActiveBlueStackWindow();
    bot:ClickAt(tab.x,tab.y);
    bot:AddTask("bot:log('onDungeonClear')",3000);
end;

function ProcessTask()
    local now = Environment.TickCount;
    --bot.lstTask:RefreshItems();
    for i=0, bot.lstTask.Items.Count -1 do
        local item = bot.lstTask.Items[i];
        if now > item.execTime then
            taskResult = false;
            local f = loadstring("taskResult = " .. item.script);
            f();
            if taskResult ~= true then
                bot.lstTask.Items:RemoveAt(i);
            end;
            return true
        end;
    end;
    return false;
end;

function Auto()

	-- bot:log(classifyResult:ToString());
	screenName = classifyResult.label;
	local screens = stateTable[currentState];
	if bot.screenClassifiers:ContainsKey(screenName) == true then
		local subScreenClassifier = bot.screenClassifiers[screenName];
		local subResult = subScreenClassifier:Classify(currentBitMap, 0.9);
		if subResult ~= nil then
			screenName = screenName .. "/" .. subResult.label;
		end;
	end;
	local features = bot:GetFeatures(currentBitMap);
	bot:log("[" .. currentState .. '] ' .. screenName);
	bot.txtScreenStatus.Text = "[" .. currentState .. '] ' .. screenName .. ' ' .. ArrayToString(features);
	bot.txtDebug:ScrollToCaret();
	if bot.chkAutoClick.Checked == true then
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

function MainClick(name)
    -- make sure that current is main and click
    if screenName ~= nil then
        bot:log("MainClick: " .. screenName);
        if string.starts(screenName,"Main") then
        --if screenName == "Main" then 
            Click(name)
            return false;
        end;
    end;
    -- continue     
    return true;
end;

function AutoQuest()
    currentState = "AutoQuest";
    if noMoreQuest == true then
        bot:log("Finish All Quest Item");
        if noMoreWeeklyQuest == true then
            bot:log("Finish All Weekley Quest");
            return;
        end;
        -- check weekly quest
        Click("Quest");
        return;
    end;

    bot:AddTask("MainClick('Bag')",0);
end;

clickPoint = {};
clickPoint["Bag"] = {x=984,y=34};
clickPoint["BagExit"] = {x=1242,y=38};
clickPoint["Quest"] = {x=930,y=90};

function Click(name)
    if clickPoint[name] ~= nil then
        local tab = clickPoint[name];
        bot:ClickAt(tab.x,tab.y);
    end;
end;

stateTable = {};
stateTable["Auto"] = {
    {screen="QuestSkip",cmd=ClickAt,x=1180,y=510},
    {screen="QuestComplete",cmd=ClickAt,x=634,y=615},
    {screen="QuestDo",cmd=ClickAt,x=763,y=613},
    {screen="QuestAccept",cmd=ClickAt,x=763,y=613},
    {screen="Move",cmd=ClickAt,x=511,y=519},
    {screen="Dead",
		cmd=function(tab)
			currentState = "Dead";
			bot:ClickAt(1234,252);
        end;
	},
    {screen="PartyRequest",cmd=ClickAt,x=900,y=56},
    {screen="HarvestExit",cmd=ClickAt,x=760,y=482},
--    {screen="QuestSelectB2",cmd=ClickAt,x=705,y=385},
--    {screen="QuestSelectB2",cmd=ClickAt,x=550,y=385},
    {screen="QuestSelectB2",cmd=ClickAt,x=500,y=300},
--    {screen="QuestSelectB3",cmd=ClickAt,x=380,y=385}, --222},
    {screen="QuestSelectB3",cmd=ClickAt,x=850,y=222}, --222},
    {screen="QuestSelected",cmd=ClickAt,x=900,y=650},
    {screen="QuestConfirm",cmd=ClickAt,x=760,y=500},
    {screen="ClearDungeon",cmd=onDungeonClear,x=956,y=635},
    {screen="DungeonEXP/NoMore",cmd=ClickAt,x=1240,y=40},   -- Exit
    {screen="DungeonGold/CanDo",cmd=ClickAt,x=1080,y=640},   
    


    
    {screen="WeeklyQuest/DoQuest",cmd=ClickAt,x=940,y=420},
    {screen="WeeklyQuest/QuestSuccess",cmd=ClickAt,x=937,y=423},
    {screen="WeeklyQuest/MoveNow",cmd=ClickAt,x=940,y=420},
    {screen="WeeklyQuest/NoQuest",
        cmd=function(tab)
            noMoreWeeklyQuest = true;
            ClickAt(tab);
        end,
    x=1242,y=40},
    
    {screen="Main/Success",cmd=ClickAt,x=260,y=330},
    {screen="Main/Success2",cmd=ClickAt,x=260,y=407},
    {screen="Main/Harvest",cmd=ClickAt,x=689,y=262},
    {screen="Main/HarvestDone",cmd=ClickAt,x=800,y=84},
--    {screen="Main/PartyAutoNoSkill",cmd=ClickAt,x=874,y=683},
--    {screen="Main/Quest2",cmd=ClickAt,x=52,y=312},
    {screen="Main/Quest2",cmd=ClickAt,x=91,y=400},
	{screen="Main/Quest3",cmd=ClickAt,x=91,y=320},
    {screen="Login",cmd=ClickAt,x=625,y=610},
    {screen="Ads",cmd=ClickAt,x=1257,y=21},
    {screen="CharSelect",cmd=ClickAt,x=1079,y=647},
    
    {screen="CanNotReset",cmd=onCanNotReset,x=635,y=485},    
};

stateTable["Dead"] = {
	{screen="Main",
		cmd=function(tab)
			currentState = "Auto";
			bot:ClickAt(91,320);
        end;

	},
};

stateTable["AutoQuest"] = {
    
    {screen="Main/Success",cmd=ClickAt,x=260,y=330},
    {screen="Main/Harvest",cmd=ClickAt,x=689,y=262},
    {screen="Main/HarvestDone",cmd=ClickAt,x=863,y=84},
--    {screen="Main/PartyAutoNoSkill",cmd=ClickAt,x=874,y=683},
    {screen="Main/Quest2",cmd=ClickAt,x=52,y=312},
    
    {screen="Bag",cmd=ClickAt,x=1145,y=130},
    {screen="Bag/item3",cmd=ClickAt,x=833,y=471},
    {screen="Bag/questItem",
        cmd=function(tab)
            if noMoreQuest == true then
                -- exit
                Click("BagExit");
                currentState = "Auto";
            else
                bot:ClickAt(705,397);
            end;
        end;
    },
    {screen="CanNotReset",
        cmd = function(tab)
            noMoreQuest = true;
            bot:ClickAt(635,485);
        end,
    },
    {screen="NoMoreQuest",
        cmd = function(tab)
            bot:log("onNoMoreQuest");
            bot:ClickAt(635,485);
        end,
    },    
    {screen="ResetConfirm",
        cmd = function(tab)
            bot:log("onResetConfirm");
            currentState = "JustResetQuest";
        end,
    },    
    {screen="Quest/Main",
        cmd = function(tab)
            -- check at weekly
            bot:ClickAt(120,290);
            currentState = "Auto";
        end,
    },       
};

stateTable["JustResetQuest"] = {
    {screen="ResetConfirm",cmd=ClickAt,x=750,y=490},    
    {screen="Bag/questItem",
        cmd=function(tab)
            Click("BagExit");
            currentState = "Auto";
        end,
    },    
}
if currentState == nil then
    currentState = "Auto";
end;



bot:log("Main.lua loaded");