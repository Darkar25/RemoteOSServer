const opencomputersLangMapperTable = {
	material: {
		"Meta=0": "item.oc.cuttingwire",
		"Meta=1": "item.oc.acid",
		"Meta=2": "item.oc.rawcircuitboard",
		"Meta=3": "item.oc.circuitboard",
		"Meta=4": "item.oc.printedcircuitboard",
		"Meta=5": "item.oc.cardbase",
		"Meta=6": "item.oc.transistor",
		"Meta=7": "item.oc.microchip0",
		"Meta=8": "item.oc.microchip1",
		"Meta=9": "item.oc.microchip2",
		"Meta=10": "item.oc.alu",
		"Meta=11": "item.oc.controlunit",
		"Meta=12": "item.oc.disk",
		"Meta=13": "item.oc.interweb",
		"Meta=14": "item.oc.buttongroup",
		"Meta=15": "item.oc.arrowkeys",
		"Meta=16": "item.oc.numpad",
		"Meta=17": "item.oc.tabletcase0",
		"Meta=18": "item.oc.tabletcase1",
		"Meta=19": "item.oc.tabletcase3",
		"Meta=20": "item.oc.microcontrollercase0",
		"Meta=21": "item.oc.microcontrollercase1",
		"Meta=22": "item.oc.microcontrollercase3",
		"Meta=23": "item.oc.dronecase0",
		"Meta=24": "item.oc.dronecase1",
		"Meta=25": "item.oc.dronecase3",
		"Meta=26": "item.oc.inkcartridgeempty",
		"Meta=27": "item.oc.inkcartridge",
		"Meta=28": "item.oc.chamelium",
		"Meta=29": "item.oc.diamondchip"
	},
	upgrade: {
		"Meta=0": "item.oc.upgradeangel",
		"Meta=1": "item.oc.upgradebattery0",
		"Meta=2": "item.oc.upgradebattery1",
		"Meta=3": "item.oc.upgradebattery2",
		"Meta=4": "item.oc.upgradechunkloader",
		"Meta=5": "item.oc.upgradecontainercard0",
		"Meta=6": "item.oc.upgradecontainercard1",
		"Meta=7": "item.oc.upgradecontainercard2",
		"Meta=8": "item.oc.upgradecontainerupgrade0",
		"Meta=9": "item.oc.upgradecontainerupgrade1",
		"Meta=10": "item.oc.upgradecontainerupgrade2",
		"Meta=11": "item.oc.upgradecrafting",
		"Meta=12": "item.oc.upgradedatabase0",
		"Meta=13": "item.oc.upgradedatabase1",
		"Meta=14": "item.oc.upgradedatabase2",
		"Meta=15": "item.oc.upgradeexperience",
		"Meta=16": "item.oc.upgradegenerator",
		"Meta=17": "item.oc.upgradeinventory",
		"Meta=18": "item.oc.upgradeinventorycontroller",
		"Meta=19": "item.oc.upgradenavigation",
		"Meta=20": "item.oc.upgradepiston",
		"Meta=21": "item.oc.upgradesign",
		"Meta=22": "item.oc.upgradesolargenerator",
		"Meta=23": "item.oc.upgradetank",
		"Meta=24": "item.oc.upgradetankcontroller",
		"Meta=25": "item.oc.upgradetractorbeam",
		"Meta=26": "item.oc.upgradeleash",
		"Meta=27": "item.oc.upgradehover0",
		"Meta=28": "item.oc.upgradehover1",
		"Meta=29": "item.oc.upgradetrading",
		"Meta=30": "item.oc.upgrademf",
		"Meta=31": "item.oc.wirelessnetworkcard0",
		"Meta=32": "item.oc.componentbus3",
		"Meta=33": "item.oc.upgradestickypiston"
	},
	storage: {
		"Meta=0": "item.oc.eeprom",
		"Meta=1": "item.oc.floppydisk",
		"Meta=2": "item.oc.harddiskdrive0",
		"Meta=3": "item.oc.harddiskdrive1",
		"Meta=4": "item.oc.harddiskdrive2"
	},
	card: {
		"Meta=0": "item.oc.debugcard",
		"Meta=1": "item.oc.graphicscard0",
		"Meta=2": "item.oc.graphicscard1",
		"Meta=3": "item.oc.graphicscard2",
		"Meta=4": "item.oc.redstonecard0",
		"Meta=5": "item.oc.redstonecard1",
		"Meta=6": "item.oc.networkcard",
		"Meta=7": "item.oc.wirelessnetworkcard1",
		"Meta=8": "item.oc.internetcard",
		"Meta=9": "item.oc.linkedcard",
		"Meta=10": "item.oc.datacard0",
		"Meta=11": "item.oc.datacard1",
		"Meta=12": "item.oc.datacard2"
	},
	component: {
		"Meta=0": "item.oc.cpu0",
		"Meta=1": "item.oc.cpu1",
		"Meta=2": "item.oc.cpu2",
		"Meta=3": "item.oc.componentbus0",
		"Meta=4": "item.oc.componentbus1",
		"Meta=5": "item.oc.componentbus2",
		"Meta=6": "item.oc.memory0",
		"Meta=7": "item.oc.memory1",
		"Meta=8": "item.oc.memory2",
		"Meta=9": "item.oc.memory3",
		"Meta=10": "item.oc.memory4",
		"Meta=11": "item.oc.memory5",
		"Meta=12": "item.oc.server3",
		"Meta=13": "item.oc.server0",
		"Meta=14": "item.oc.server1",
		"Meta=15": "item.oc.server2",
		"Meta=16": "item.oc.apu0",
		"Meta=17": "item.oc.apu1",
		"Meta=18": "item.oc.apu2",
		"Meta=19": "item.oc.terminalserver",
		"Meta=20": "item.oc.diskdrivemountable"
	},
	tool: {
		"Meta=0": "item.oc.analyzer",
		"Meta=1": "item.oc.debugger",
		"Meta=2": "item.oc.terminal",
		"Meta=3": "item.oc.texturepicker",
		"Meta=4": "item.oc.manual",
		"Meta=5": "item.oc.nanomachines"
	}
};

langMappers.push((obj) => {
	if(obj.ModName != "opencomputers") return undefined;
	if(opencomputersLangMapperTable[obj.Name]) {
		if(typeof opencomputersLangMapperTable[obj.Name] == "string")
			return opencomputersLangMapperTable[obj.Name];
		else {
			for (const [key, value] of Object.entries(opencomputersLangMapperTable[obj.Name])) {
				var props = key.split(',');
				var match = true;
				for (const val of props) {
					var entry = val.split('=');
					match = match && String(obj[entry[0]]) == entry[1];
					if(!match) break;
				}
				if(match) return value;
			}
		}
	}
	return (obj.Count ? "item" : "tile") + ".oc." + obj.Name;
});