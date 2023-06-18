window.minecraftLangMapperTable = {
	stone: {
		smooth_granite: "tile.stone.graniteSmooth",
		smooth_diorite: "tile.stone.dioriteSmooth",
		smooth_andesite: "tile.stone.andesiteSmooth"
	},
	planks: "tile.wood.{variant}",
	sand: {
		red_sand: "tile.sand.red"
	},
	sandstone: {
		"type=chiseled_sandstone": "tile.sandStone.chiseled",
		"type=smooth_sandstone": "tile.sandStone.smooth"
	},
	red_sandstone: {
		"type=chiseled_red_sandstone": "tile.redSandStone.chiseled",
		"type=smooth_red_sandstone": "tile.redSandStone.smooth"
	},
	gold_ore: "tile.oreGold",
	iron_ore: "tile.oreIron",
	coal_ore: "tile.oreCoal",
	stained_glass: "tile.stainedGlass.{color}",
	stained_glass_pane: "tile.thinStainedGlass.{color}",
	wool: "tile.cloth.{color}",
	gold_block: "tile.blockGold",
	iron_block: "tile.blockIron",
	wooden_slab: "tile.woodSlab.{variant}",
	brick_block: "tile.brick",
	oak_stairs: "tile.stairsWood",
	spruce_stairs: "tile.stairsWoodSpruce",
	birch_stairs: "tile.stairsWoodBirch",
	jungle_stairs: "tile.stairsWoodJungle",
	acacia_stairs: "tile.stairsWoodAcacia",
	dark_oak_stairs: "tile.stairsWoodDarkOak",
	redstone_wire: "tile.redstoneDust",
	diamond_ore: "tile.oreDiamond",
	coal_block: "tile.blockCoal",
	diamond_block: "tile.blockDiamond",
	crafting_table: "tile.workbench",
	wooden_door: "tile.doorWood",
	stone_stairs: "tile.stairsStone",
	sandstone_stairs: "tile.stairsSandStone",
	red_sandstone_stairs: "tile.stairsRedSandStone",
	stone_pressure_plate: "tile.pressurePlateStone",
	wooden_pressure_plate: "tile.pressurePlateWood",
	light_weighted_pressure_plate: "tile.weightedPlate_light",
	heavy_weighted_pressure_plate: "tile.weightedPlate_heavy",
	iron_door: "tile.doorIron",
	redstone_ore: "tile.oreRedstone",
	redstone_torch: "tile.notGate",
	carpet: "tile.woolCarpet.{color}",
	packed_ice: "tile.icePacked",
	stained_hardened_clay: "tile.clayHardenedStained.{color}",
	netherrack: "tile.hellrock",
	soul_sand: "tile.hellsand",
	glowstone: "tile.lightgem",
	lapis_ore: "tile.oreLapis",
	lapis_block: "tile.blockLapis",
	stonebrick: {
		stonebrick: "tile.stonebricksmooth",
		mossy_stonebrick: "tile.stonebricksmooth.mossy",
		cracked_stonebrick: "tile.stonebricksmooth.cracked",
		chiseled_stonebrick: "tile.stonebricksmooth.chiseled"
	},
	monsted_egg: {
		stone: "tile.monsterStoneEgg",
		cobblestone: "tile.monsterStoneEgg.cobble",
		stonebrick: "tile.monsterStoneEgg.brick",
		mossy_stonebrick: "tile.monsterStoneEgg.mossybrick",
		cracked_stonebrick: "tile.monsterStoneEgg.crackedbrick",
		chiseled_stonebrick: "tile.monsterStoneEgg.chiseledbrick"
	},
	piston: "tile.pistonBase",
	sticky_piston: "tile.pistonStickyBase",
	iron_bars: "tile.fenceIron",
	brick_stairs: "tile.stairsBrick",
	stone_brick_stairs: "tile.stairsStoneBrickSmooth",
	nether_brick_fence: "tile.netherFence",
	nether_brick_stairs: "tile.stairsNetherBrick",
	nether_wart: "tile.netherStalk",
	enchanting_table: "tile.enchantmentTable",
	anvil: {
		"damage=0": "tile.anvil.intact",
		"damage=1": "tile.anvil.slightlyDamaged",
		"damage=2": "tile.anvil.veryDamaged"
	},
	end_stone: "tile.whiteStone",
	mycelium: "tile.mycel",
	redstone_lamp: "tile.redstoneLight",
	emerald_ore: "tile.oreEmerald",
	emerald_block: "tile.blockEmerald",
	redstone_block: "tile.blockRedstone",
	tripwire_hook: "tile.tripWireSource",
	cobblestone_wall: {
		cobblestone: "tile.cobbleWall.normal",
		mossy_cobblestone: "tile.cobbleWall.mossy"
	},
	quartz_block: {
		lines_y: "tile.quartzBlock.lines",
	},
	quartz_stairs: "tile.stairsQuartz",
	prismarine: {
		prismarine: "tile.prismarine.rough",
		prismarine_bricks: "tile.prismarine.bricks",
		dark_prismarine: "tile.prismarine.dark",
	},
	purpur_stairs: "tile.stairsPurpur",
	white_glazed_terracotta: "tile.glazedTerracottaWhite",
	orange_glazed_terracotta: "tile.glazedTerracottaOrange",
	magenta_glazed_terracotta: "tile.glazedTerracottaMagenta",
	light_blue_glazed_terracotta: "tile.glazedTerracottaLightBlue",
	yellow_glazed_terracotta: "tile.glazedTerracottaYellow",
	lime_glazed_terracotta: "tile.glazedTerracottaLime",
	pink_glazed_terracotta: "tile.glazedTerracottaPink",
	gray_glazed_terracotta: "tile.glazedTerracottaGray",
	silver_glazed_terracotta: "tile.glazedTerracottaSilver",
	cyan_glazed_terracotta: "tile.glazedTerracottaCyan",
	purple_glazed_terracotta: "tile.glazedTerracottaPurple",
	blue_glazed_terracotta: "tile.glazedTerracottaBlue",
	brown_glazed_terracotta: "tile.glazedTerracottaBrown",
	green_glazed_terracotta: "tile.glazedTerracottaGreen",
	red_glazed_terracotta: "tile.glazedTerracottaRed",
	black_glazed_terracotta: "tile.glazedTerracottaBlack",
	concrete: "tile.concrete.{color}",
	concrete_powder: "tile.concretePowder.{color}",
	dirt: {
		dirt: "tile.dirt",
		coarse_dirt: "tile.dirt.coarse"
	},
};

langMappers.push((obj) => {
	if(obj.ModName != "minecraft") return undefined;
	if(minecraftLangMapperTable[obj.Name]) {
		var processed = "";
		if(typeof minecraftLangMapperTable[obj.Name] == "string") {
			processed = minecraftLangMapperTable[obj.Name];
		} else {
			for (const [key, value] of Object.entries(minecraftLangMapperTable[obj.Name])) {
				var props = key.split(',');
				var match = true;
				for (const val of props) {
					var entry = val.split('=');
					if(entry.length == 1) match = match && String(obj.Properties["variant"] ?? "normal") == entry[0];
					else match = match && String(obj.Properties[entry[0]] ?? obj[entry[0]]) == entry[1];
					if(!match) break;
				}
				if(match) {
					processed = value;
					break;
				}
			}
		}
		if(processed) return processed.replace(/\{(.*?)}/g, (x, prop)=> obj.Properties[prop] ?? obj[prop]);
	}
	return (obj.Count ? "item" : "tile") + "." + obj.Name.replace("/\_/g", "") +(obj.Properties.variant ? ("." + obj.Properties.variant) : "");
});