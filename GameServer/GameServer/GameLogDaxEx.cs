using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GameServer;

public static class GameLogDaxEx
{
	public static List<SqlCommand> CSC_AddHeroCreatureCreationLog(HeroCreature heroCreature, DateTimeOffset currentTime)
	{
		List<SqlCommand> sqlCommands = new List<SqlCommand>();
		sqlCommands.Add(GameLogDac.CSC_AddHeroCreatureCreationLog(heroCreature.instanceId, heroCreature.hero.id, heroCreature.creature.id, heroCreature.quality, currentTime));
		foreach (HeroCreatureBaseAttr baseAttr in heroCreature.baseAttrs.Values)
		{
			sqlCommands.Add(GameLogDac.CSC_AddHeroCreatureCreationBaseAttrLog(baseAttr.creature.instanceId, baseAttr.attr.attr.attrId, baseAttr.baseValue));
		}
		foreach (HeroCreatureAdditionalAttr additionalAttr in heroCreature.additionalAttrs)
		{
			sqlCommands.Add(GameLogDac.CSC_AddHeroCreatureCreationAdditionalAttrLog(additionalAttr.creature.instanceId, additionalAttr.attr.attrId));
		}
		HeroCreatureSkill[] skills = heroCreature.skills;
		foreach (HeroCreatureSkill skill in skills)
		{
			if (skill.skillAttr != null)
			{
				sqlCommands.Add(GameLogDac.CSC_AddHeroCreatureCreationSkillLog(skill.creature.instanceId, skill.slotIndex, skill.skillAttr.skill.id, skill.skillAttr.grade.grade));
			}
		}
		return sqlCommands;
	}
}
