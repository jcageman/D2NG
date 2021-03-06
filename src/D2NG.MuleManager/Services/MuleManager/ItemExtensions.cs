﻿using D2NG.Core.D2GS.Items;
using D2NG.Core.MCP;
using D2NG.MuleManager.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace D2NG.MuleManager.Services.MuleManager
{
    public static class ItemExtensions
    {
        public static MuleItem MapToMuleItem(this Item item, MuleManagerAccount account, Character character)
        {
            return new MuleItem
            {
                Id = $"{account.Name}-{character.Name}-{item.Id}",
                AccountName = account.Name,
                CharacterName = character.Name,
                ItemName = item.Name.ToString(),
                QualityType = item.Quality.ToString(),
                ClassificationType = item.Classification.ToString(),
                Stats = item.Properties.ToDictionary(k => k.MapToStatKey(), v => v.Value.Value)
            };
        }

        public static string MapToStatKey(this KeyValuePair<StatType, ItemProperty> statTypeAndProperty)
        {
            switch (statTypeAndProperty.Key)
            {
                case StatType.SingleSkill1:
                case StatType.SingleSkill2:
                case StatType.SingleSkill3:
                case StatType.SingleSkill4:
                    return statTypeAndProperty.Value.Skill.ToString();
                default:
                    return statTypeAndProperty.Key.ToString();
            }
        }
    }
}
