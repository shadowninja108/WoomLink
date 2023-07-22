using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WoomLink.xlink2.Properties;
using WoomLink.xlink2.Properties.Enum;

namespace WoomLink.Ex
{
    public class PropertyParser
    {
        public class EnumTypeDefinition
        {
            public string Name;
            public IList<EnumDefinition> Enums;

            public static EnumTypeDefinition Parse(StreamReader reader, string name)
            {
                EnumTypeDefinition res = new()
                {
                    Name = name,
                    Enums = new List<EnumDefinition>()
                };

                bool foundEnd = false;
                while(!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Trim();

                    if (line == "};" || line == "}")
                    {
                        foundEnd = true;
                        break;
                    }

                    /* Trim off the comma at the end if needed. */
                    if (line.EndsWith(','))
                        line = line[..^1];

                    /* Extract values. */
                    var split = line.Split('=');
                    split[0] = split[0].Trim();
                    split[1] = split[1].Trim();

                    res.Enums.Add(new EnumDefinition()
                    {
                        Name = split[0],
                        Value = Utils.ParseInt(split[1])
                    });
                }

                if (reader.EndOfStream && !foundEnd)
                {
                    throw new Exception($"Incomplete enum {name}");
                }

                return res;
            }
        }

        public static PropertyDefinition[] Parse(Stream stream)
        {
            StreamReader reader = new(stream);

            List<PropertyDefinition> properties = new();
            List<EnumTypeDefinition> definedEnums = new();

            void AddProperty(PropertyDefinition prop)
            {
                if (properties.Any(x => x.Name == prop.Name))
                    throw new Exception($"Duplicate property {prop.Name}");
                properties.Add(prop);
            }

            void AddEnum(EnumTypeDefinition enu)
            {
                if (properties.Any(x => x.Name == enu.Name))
                    throw new Exception($"Duplicate enum {enu.Name}");
                definedEnums.Add(enu);
            }

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine().Trim();
                var splitBySpaces = line.Split(null);

                if(line == string.Empty)
                    continue;

                /* Trim off ; */
                var name = splitBySpaces[1];
                if (name.EndsWith(';'))
                    name = name[..^1];

                if (line.StartsWith("enum"))
                {
                    AddEnum(EnumTypeDefinition.Parse(reader, name));
                    continue;
                }

                if (line.StartsWith("float"))
                {
                    AddProperty(new F32PropertyDefinition
                    {
                        Name = name
                    });
                    continue;
                }

                if (line.StartsWith("int"))
                {
                    AddProperty(new S32PropertyDefinition
                    {
                        Name = name
                    });
                    continue;
                }

                var matchingEnum = definedEnums.FirstOrDefault(x => x.Name == splitBySpaces[0]);

                if (matchingEnum != null)
                {
                    AddProperty(new EnumPropertyDefinition()
                    {
                        Name = name,
                        TypeName = matchingEnum.Name,
                        Enums = matchingEnum.Enums.ToList()
                    });
                    continue;
                }

                throw new Exception("Invalid line");
            }

            return properties.ToArray();
        }
    }
}
