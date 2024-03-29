﻿using System.Collections.Generic;

namespace Core.Configurations;

public abstract class ConfigurationItem
{
   public abstract string Key { get; }

   public abstract void SetItem(string key, ConfigurationItem item);

   public abstract void RemoveItem(string key);

   public abstract IEnumerable<(string key, string text)> Items();

   public abstract IEnumerable<(string key, Setting setting)> Settings();

   public abstract int Count { get; }

   public ConfigurationMaybe Maybe => new((IConfigurationItemGetter)this);

   public ConfigurationResult Result => new((IConfigurationItemGetter)this);

   public ConfigurationOptional Optional => new((IConfigurationItemGetter)this);

   public ConfigurationValue Value => new((IConfigurationItemGetter)this);

   public ConfigurationRequired Required => new(this);

   public abstract ConfigurationItem Clone();

   public abstract ConfigurationItem Clone(string key);
}