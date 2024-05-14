// <copyright file="ImportHelper.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Data.Import
{
	using System;
	using System.Linq;
	using IFBN.Data.Contants;
	using IFBN.Data.Entities;

	public class ImportHelper
	{
		public IFBNContext Context { get; set; }

		public Measurement AddAndGetMeasurement(Census census, string type, string unit, string method = Methods.Unknown)
		{
			TypeUnit typeUnit = GetTypeUnit(type, unit);
			TypeMethod typeMethod = GetTypeMethod(type, method);

			if (typeUnit == null || typeMethod == null)
			{
				throw new InvalidOperationException();
			}

			Measurement measurement = new Measurement { TypeUnit = typeUnit, TypeMethod = typeMethod };
			census.Measurements.Add(measurement);

			return measurement;
		}

		public void AddIfMissingCode(string name)
		{
			Code type = GetCode(name);

			if (type == null)
			{
				Context.Codes.Add(new Code() { Name = name });
			}
		}

		public void AddIfMissingInstitution(Feature feature, string name, bool isNetwork = false)
		{
			Institution institution = GetInstitution(name);

			if (institution == null)
			{
				institution = new Institution() { Name = name };
				Context.Institutions.Add(institution);
			}

			if (isNetwork)
			{
				feature.Network = institution.Name;
			}
			else
			{
				FeatureInstitution featureInstitution = GetFeatureInstitution(feature, institution);

				if (featureInstitution == null)
				{
					Context.FeatureInstitutions.Add(new FeatureInstitution() { Feature = feature, Institution = institution, });
				}
			}
		}

		public void AddIfMissingMethod(string name)
		{
			Method method = GetMethod(name);

			if (method == null)
			{
				Context.Methods.Add(new Method() { Name = name });
			}
		}

		public void AddIfMissingPlotType(string name)
		{
			PlotType type = GetPlotType(name);

			if (type == null)
			{
				Context.PlotTypes.Add(new PlotType() { Name = name });
			}
		}

		public void AddIfMissingPrincipalInvestigator(Feature feature, string name)
		{
			PrincipalInvestigator principalInvestigator = GetPrincipalInvestigator(name);

			if (principalInvestigator == null)
			{
				principalInvestigator = new PrincipalInvestigator() { Name = name };
				Context.PrincipalInvestigators.Add(principalInvestigator);
			}

			FeaturePrincipalInvestigator featurePI = GetFeaturePrincipalInvestigator(feature, principalInvestigator);

			if (featurePI == null)
			{
				Context.FeaturePrincipalInvestigators.Add(new FeaturePrincipalInvestigator()
				{
					Feature = feature, PrincipalInvestigator = principalInvestigator,
				});
			}
		}

		public void AddIfMissingType(string name)
		{
			Entities.Type type = GetType(name);

			if (type == null)
			{
				Context.Types.Add(new Entities.Type() { Name = name });
			}
		}

		public void AddIfMissingTypeCode(string typeName, string codeName)
		{
			Entities.TypeCode typeCode = GetTypeCode(typeName, codeName);

			if (typeCode == null)
			{
				Context.TypeCodes.Add(new Entities.TypeCode() { Type = GetType(typeName), Code = GetCode(codeName) });
			}
		}

		public void AddIfMissingTypeMethod(string typeName, string methodName = Methods.Unknown)
		{
			TypeMethod typeMethod = GetTypeMethod(typeName, methodName);

			if (typeMethod == null)
			{
				Context.TypeMethods.Add(new TypeMethod() { Type = GetType(typeName), Method = GetMethod(methodName) });
			}
		}

		public void AddIfMissingTypeUnit(string typeName, string unitName)
		{
			TypeUnit typeUnit = GetTypeUnit(typeName, unitName);

			if (typeUnit == null)
			{
				Context.TypeUnits.Add(new TypeUnit() { Type = GetType(typeName), Unit = GetUnit(unitName) });
			}
		}

		public void AddIfMissingUnit(string name, string symbol)
		{
			Unit type = GetUnit(name);

			if (type == null)
			{
				Context.Units.Add(new Unit() { Name = name, Symbol = symbol });
			}
		}

		public void AddMeasurement(Census census, string type, string unit, double value, string method = "Unknown")
		{
			if (value == 0)
			{
				return;
			}

			Measurement measurement = AddAndGetMeasurement(census, type, unit, method);
			measurement.ValueFloat = value;
		}

		public void AddMeasurement(Census census, string type, string unit, int value, string method = "Unknown")
		{
			if (value == 0)
			{
				return;
			}

			Measurement measurement = AddAndGetMeasurement(census, type, unit, method);
			measurement.ValueInt = value;
		}

		public void AddMeasurementCode(Census census, string type, string unit, string code)
		{
			if (string.IsNullOrEmpty(code))
			{
				return;
			}

			Measurement measurement = AddAndGetMeasurement(census, type, unit);

			Entities.TypeCode typeCode = GetTypeCode(type, code);
			measurement.TypeCode = typeCode ?? throw new InvalidOperationException();
		}

		public void AddMissingCodes()
		{
			AddIfMissingCode(Codes.Burned);
			AddIfMissingCode(Codes.Mature);
			AddIfMissingCode(Codes.SecondaryForestYoung);
			AddIfMissingCode(Codes.SecondaryForestIntermediate);
			AddIfMissingCode(Codes.SecondaryForestOlder);
			AddIfMissingCode(Codes.SecondaryForestMaturing);
			AddIfMissingCode(Codes.OldGrowth);

			AddIfMissingTypeCode(Types.ForestStatus, Codes.Burned);
			AddIfMissingTypeCode(Types.ForestStatus, Codes.Mature);
			AddIfMissingTypeCode(Types.ForestStatus, Codes.SecondaryForestYoung);
			AddIfMissingTypeCode(Types.ForestStatus, Codes.SecondaryForestIntermediate);
			AddIfMissingTypeCode(Types.ForestStatus, Codes.SecondaryForestOlder);
			AddIfMissingTypeCode(Types.ForestStatus, Codes.SecondaryForestMaturing);
			AddIfMissingTypeCode(Types.ForestStatus, Codes.OldGrowth);
		}

		public void AddMissingMethods()
		{
			AddIfMissingMethod(Methods.Unknown);
			AddIfMissingMethod(Methods.Average);
			AddIfMissingMethod(Methods.Chave);
			AddIfMissingMethod(Methods.Feldpausch);
			AddIfMissingMethod(Methods.Local);
			AddIfMissingMethod(Methods.LowerQuantil);
			AddIfMissingMethod(Methods.UpperQuantil);
		}

		public void AddMissingPlotTypes()
		{
			AddIfMissingPlotType(PlotTypes.Site);
			AddIfMissingPlotType(PlotTypes.Plot);
		}

		public void AddMissingTypeMethods()
		{
			// TODO: Define Methods for measurements
			AddIfMissingTypeMethod(Types.Altitude);
			AddIfMissingTypeMethod(Types.Area);
			AddIfMissingTypeMethod(Types.Slope);
			AddIfMissingTypeMethod(Types.HAverage);
			AddIfMissingTypeMethod(Types.HMax);
			AddIfMissingTypeMethod(Types.HMax, Methods.Chave);
			AddIfMissingTypeMethod(Types.HMax, Methods.Feldpausch);
			AddIfMissingTypeMethod(Types.HMax, Methods.Local);
			AddIfMissingTypeMethod(Types.HLorey, Methods.Chave);
			AddIfMissingTypeMethod(Types.HLorey, Methods.Feldpausch);
			AddIfMissingTypeMethod(Types.HLorey, Methods.Local);
			AddIfMissingTypeMethod(Types.MinDBH);
			AddIfMissingTypeMethod(Types.AGBLocalHD);
			AddIfMissingTypeMethod(Types.AGBLocalHD, Methods.Average);
			AddIfMissingTypeMethod(Types.AGBLocalHD, Methods.LowerQuantil);
			AddIfMissingTypeMethod(Types.AGBLocalHD, Methods.UpperQuantil);
			AddIfMissingTypeMethod(Types.AGBFeldpausch);
			AddIfMissingTypeMethod(Types.AGBFeldpausch, Methods.Average);
			AddIfMissingTypeMethod(Types.AGBFeldpausch, Methods.LowerQuantil);
			AddIfMissingTypeMethod(Types.AGBFeldpausch, Methods.UpperQuantil);
			AddIfMissingTypeMethod(Types.AGBChave);
			AddIfMissingTypeMethod(Types.AGBChave, Methods.Average);
			AddIfMissingTypeMethod(Types.AGBChave, Methods.LowerQuantil);
			AddIfMissingTypeMethod(Types.AGBChave, Methods.UpperQuantil);
			AddIfMissingTypeMethod(Types.WoodDensity);
			AddIfMissingTypeMethod(Types.ForestStatus);
			AddIfMissingTypeMethod(Types.BasalArea);
			AddIfMissingTypeMethod(Types.BasalArea);
			AddIfMissingTypeMethod(Types.GSV);
			AddIfMissingTypeMethod(Types.Ndens);
		}

		public void AddMissingTypes()
		{
			AddIfMissingType(Types.Altitude);
			AddIfMissingType(Types.Area);
			AddIfMissingType(Types.Slope);
			AddIfMissingType(Types.HAverage);
			AddIfMissingType(Types.HMax);
			AddIfMissingType(Types.HLorey);
			AddIfMissingType(Types.MinDBH);
			AddIfMissingType(Types.AGBLocalHD);
			AddIfMissingType(Types.AGBFeldpausch);
			AddIfMissingType(Types.AGBChave);
			AddIfMissingType(Types.WoodDensity);
			AddIfMissingType(Types.ForestStatus);
			AddIfMissingType(Types.BasalArea);
			AddIfMissingType(Types.GSV);
			AddIfMissingType(Types.Ndens);
		}

		public void AddMissingTypeUnits()
		{
			AddIfMissingTypeUnit(Types.Altitude, Units.Meter);
			AddIfMissingTypeUnit(Types.Area, Units.Hectare);
			AddIfMissingTypeUnit(Types.Slope, Units.Degree);
			AddIfMissingTypeUnit(Types.HAverage, Units.Meter);
			AddIfMissingTypeUnit(Types.HMax, Units.Meter);
			AddIfMissingTypeUnit(Types.HLorey, Units.Meter);
			AddIfMissingTypeUnit(Types.MinDBH, Units.Centimeter);
			AddIfMissingTypeUnit(Types.AGBLocalHD, Units.TonsPerHectare);
			AddIfMissingTypeUnit(Types.AGBFeldpausch, Units.TonsPerHectare);
			AddIfMissingTypeUnit(Types.AGBChave, Units.TonsPerHectare);
			AddIfMissingTypeUnit(Types.WoodDensity, Units.TonsPerCubicMeter);
			AddIfMissingTypeUnit(Types.ForestStatus, Units.Unknown);
			AddIfMissingTypeUnit(Types.BasalArea, Units.SquareMeterPerHectare);
			AddIfMissingTypeUnit(Types.GSV, Units.CubicMeterPerHectare);
			AddIfMissingTypeUnit(Types.Ndens, Units.TreesPerHectare);
		}

		public void AddMissingUnits()
		{
			AddIfMissingUnit(Units.Unknown, null);
			AddIfMissingUnit(Units.Meter, "m");
			AddIfMissingUnit(Units.SqMeter, "m2");
			AddIfMissingUnit(Units.Hectare, "ha");
			AddIfMissingUnit(Units.Centimeter, "cm");
			AddIfMissingUnit(Units.Degree, "°");
			AddIfMissingUnit(Units.TonsPerHectare, "t/ha");
			AddIfMissingUnit(Units.TonsPerCubicMeter, "t/m3");
			AddIfMissingUnit(Units.SquareMeterPerHectare, "m2/ha");
			AddIfMissingUnit(Units.CubicMeterPerHectare, "m3/ha");
			AddIfMissingUnit(Units.TreesPerHectare, "trees/ha");
		}

		public Census GetAndAddIfMissingCensus(Feature feature, DateTime? from = null, DateTime? to = null)
		{
			Census census = GetCensus(feature, from, to);

			if (census == null)
			{
				census = new Census() { From = from, To = to, Feature = feature };
				Context.Censuses.Add(census);
			}

			return census;
		}

		public Family GetAndAddIfMissingFamily(string name)
		{
			Family family = GetFamily(name);

			if (family == null)
			{
				family = new Family() { Name = name };
				Context.Families.Add(family);
			}

			return family;
		}

		public Genus GetAndAddIfMissingGenus(Family family, string genusName)
		{
			Genus genus = GetGenus(family, genusName);

			if (genus == null)
			{
				genus = new Genus() { Family = family, Name = genusName };
				Context.Genera.Add(genus);
			}

			return genus;
		}

		public Feature GetAndAddIfMissingPlot(Feature site, string name)
		{
			Context.Entry(site).Collection(x => x.Children).Load();

			Feature feature = site.Children.SingleOrDefault(x => x.Name == name);

			if (feature == null)
			{
				feature = new Feature() { Name = name, PlotType = GetPlotType(PlotTypes.Plot), Parent = site };
				Context.Features.Add(feature);
			}

			return feature;
		}

		public Feature GetAndAddIfMissingSite(string name)
		{
			Feature feature = GetFeature(name, PlotTypes.Site);

			if (feature == null)
			{
				feature = new Feature() { Name = name, PlotType = GetPlotType(PlotTypes.Site) };
				Context.Features.Add(feature);
			}

			return feature;
		}

		public Species GetAndAddIfMissingSpecies(Genus genus, string speciesName)
		{
			Species species = GetSpecies(genus, speciesName);

			if (species == null)
			{
				species = new Species() { Genus = genus, Name = speciesName };
				Context.Species.Add(species);
			}

			return species;
		}

		public Census GetCensus(Feature feature, DateTime? @from, DateTime? to)
		{
			return Context.Censuses.Local.SingleOrDefault(x => x.From == @from && x.To == to && x.Feature == feature) ??
				Context.Censuses.SingleOrDefault(x => x.From == @from && x.To == to && x.Feature == feature);
		}

		public Code GetCode(string name)
		{
			return Context.Codes.Local.SingleOrDefault(x => x.Name == name) ?? Context.Codes.SingleOrDefault(x => x.Name == name);
		}

		public Family GetFamily(string name)
		{
			return Context.Families.Local.SingleOrDefault(x => x.Name == name) ?? Context.Families.SingleOrDefault(x => x.Name == name);
		}

		public Feature GetFeature(string name, string plotType)
		{
			return Context.Features.Local.SingleOrDefault(x => x.Name == name && x.PlotType.Name == plotType) ??
				Context.Features.SingleOrDefault(x => x.Name == name && x.PlotType.Name == plotType);
		}

		public FeatureInstitution GetFeatureInstitution(Feature feature, Institution institution)
		{
			return Context.FeatureInstitutions.Local.SingleOrDefault(x => x.Feature == feature && x.Institution == institution) ??
				Context.FeatureInstitutions.SingleOrDefault(x => x.Feature == feature && x.Institution == institution);
		}

		public FeaturePrincipalInvestigator GetFeaturePrincipalInvestigator(Feature feature, PrincipalInvestigator principalInvestigator)
		{
			return Context.FeaturePrincipalInvestigators.Local.SingleOrDefault(x =>
					x.Feature == feature && x.PrincipalInvestigator == principalInvestigator) ??
				Context.FeaturePrincipalInvestigators.SingleOrDefault(x =>
					x.Feature == feature && x.PrincipalInvestigator == principalInvestigator);
		}

		public Genus GetGenus(Family family, string genusName)
		{
			return Context.Genera.Local.SingleOrDefault(x => x.Family == family && x.Name == genusName) ??
				Context.Genera.SingleOrDefault(x => x.Family == family && x.Name == genusName);
		}

		public Institution GetInstitution(string name)
		{
			return Context.Institutions.Local.SingleOrDefault(x => x.Name == name) ??
				Context.Institutions.SingleOrDefault(x => x.Name == name);
		}

		public Census GetLastCensus(Feature feature)
		{
			return Context.Censuses.Local.OrderByDescending(x => x.From).FirstOrDefault(x => x.Feature == feature) ??
				Context.Censuses.OrderByDescending(x => x.From).FirstOrDefault(x => x.Feature == feature);
		}

		public Method GetMethod(string name)
		{
			return Context.Methods.Local.SingleOrDefault(x => x.Name == name) ?? Context.Methods.SingleOrDefault(x => x.Name == name);
		}

		public PlotType GetPlotType(string name)
		{
			return Context.PlotTypes.Local.SingleOrDefault(x => x.Name == name) ?? Context.PlotTypes.SingleOrDefault(x => x.Name == name);
		}

		public PrincipalInvestigator GetPrincipalInvestigator(string name)
		{
			return Context.PrincipalInvestigators.Local.SingleOrDefault(x => x.Name == name) ??
				Context.PrincipalInvestigators.SingleOrDefault(x => x.Name == name);
		}

		public Feature GetSite(string name)
		{
			Feature feature = GetFeature(name, PlotTypes.Site);

			return feature;
		}

		public Species GetSpecies(Genus genus, string speciesName)
		{
			return Context.Species.Local.SingleOrDefault(x => x.Genus == genus && x.Name == speciesName) ??
				Context.Species.SingleOrDefault(x => x.Genus == genus && x.Name == speciesName);
		}

		public Entities.Type GetType(string name)
		{
			return Context.Types.Local.SingleOrDefault(x => x.Name == name) ?? Context.Types.SingleOrDefault(x => x.Name == name);
		}

		public Entities.TypeCode GetTypeCode(string typeName, string codeName)
		{
			return Context.TypeCodes.Local.SingleOrDefault(x => x.Type.Name == typeName && x.Code.Name == codeName) ??
				Context.TypeCodes.SingleOrDefault(x => x.Type.Name == typeName && x.Code.Name == codeName);
		}

		public TypeMethod GetTypeMethod(string typeName, string methodName)
		{
			return Context.TypeMethods.Local.SingleOrDefault(x => x.Type.Name == typeName && x.Method.Name == methodName) ??
				Context.TypeMethods.SingleOrDefault(x => x.Type.Name == typeName && x.Method.Name == methodName);
		}

		public TypeUnit GetTypeUnit(string typeName, string unitName)
		{
			return Context.TypeUnits.Local.SingleOrDefault(x => x.Type.Name == typeName && x.Unit.Name == unitName) ??
				Context.TypeUnits.SingleOrDefault(x => x.Type.Name == typeName && x.Unit.Name == unitName);
		}

		public Unit GetUnit(string name)
		{
			return Context.Units.Local.SingleOrDefault(x => x.Name == name) ?? Context.Units.SingleOrDefault(x => x.Name == name);
		}
	}
}