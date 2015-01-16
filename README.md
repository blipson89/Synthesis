Synthesis
=========

Synthesis is an object mapping framework for Sitecore that enables developing more reliable and maintainable sites in less time than traditional Sitecore development. It is a strongly typed template object generator that is easily understandable for developers with either a Sitecore or traditional .NET background. It neatly integrates with Sitecore MVC (via the `Synthesis.Mvc` package) as a View rendering model provider.

### Show me the code!

	// convert a Sitecore.Data.Item to its Synthesis equivalent
	var foo = Sitecore.Context.Item.As<ISampleItemItem>();

	// get page-editor values
	var pageEditor = foo.Title.RenderedValue;

	// full metadata and traversal support
	var parent = foo.Axes.Parent; // and this parent (and other axes results) is also a Synthesis object
	var modified = foo.Statistics.Updated;

	// add a new subitem
	var editing = foo.Add<ISampleItemItem>("Hello world");

	// cast items to their base interfaces at will
	var standardTemplate = (IStandardTemplateItem) foo;
	
	// you can also convert items without specifying an output type (the instance will be the most specific template type available)
	var generic = Sitecore.Context.Item.AsStronglyTyped();
	
	var type = generic.GetType().FullName; // this will be a "SampleItem" concrete instance, which : ISampleItemItem, which : IStandardTemplateItem
	
	// there are also helpers to convert collections
	var collection = Sitecore.Context.Item.Children.AsStronglyTypedCollection();
	
	// go nuts with LINQ and Sitecore 7 and query on _interfaces_
	using (var context = ContentSearchManager.CreateSearchContext(new SitecoreIndexableItem(Sitecore.Context.Item)))
	{
		// note: GetSynthesisQueryable by default automatically filters your query by:
		// - context language
		// - correct template ID for the synthesis type requested
		// - latest version of the item only
		// ...so you don't have to remember to do it :)
		var results = context.GetSynthesisQueryable<ISampleItemItem>()
							.FacetOn(x => x.Title.RawValue)
							.Take(10)
							.GetResults();
		
		ISampleItemItem exampleResult = results.Hits.First().Document;
		
		// if you have the title field indexed with value, this will grab the value out of Lucene without any database work
		var luceneString = exampleResult.Title.RawValue;
		
		// but this value isn't stored in the index. Accessing it will transparently cause the Sitecore.Data.Item to be loaded, and the value retrieved. Nice huh?
		var promoted = exampleResult.Text.ExpandedLinksValue;
	}
	
	// get crazy with LINQ queries against Synthesis items
	using (var context = ContentSearchManager.CreateSearchContext(new SitecoreIndexableItem(Sitecore.Context.Item)))
	{
		var results = context.GetSynthesisQueryable<ISampleItemItem>()
							.Where(x => x.Integer.Value == 16 &&
										x.Double.Value == 16.67m &&
										x.SingleLineText.RawValue.Contains("line") &&
										x.RichText.RawValue.StartsWith("richtext") &&
										x.Boolean.Value == true &&
										x.DateTime.Value < new DateTime(2013, 5, 1) &&
										x.Droptree.TargetId == new ID("{9D6120C6-79C1-47D4-9DD8-94E91121A2EC}") &&
										x.Multilist.TargetIds.Contains(new ID("{016A31AD-0195-4AC6-8218-5977A1C54EBB}")))
							// you'll want this clause if you disable auto filtering to avoid getting nulls in your results if the template is incorrect
							.Where(x => x.TemplateIds.Contains(Trample.ItemTemplateId))
							// you can query on arbitrary index fields, or get their values, using the indexer on Synthesis items
							.Where(x => x["_latestversion"] == "1")
							.FacetOn(x => x.Name).FacetOn(x => x.Multilist.TargetIds)
							.ToList();
		
		ISampleItemItem exampleResult = results.Hits.First().Document;
		
		// if you have the title field indexed with value, this will grab the value out of Lucene without any database work
		var luceneString = exampleResult.Title.RawValue;
		
		// but this value isn't stored in the index. Accessing it will transparently cause the Sitecore.Data.Item to be loaded, and the value retrieved. Nice huh?
		var promoted = exampleResult.Text.ExpandedLinksValue;
	}
  
Ready to try it? Get the package off NuGet and have fun! See the docs in the wiki for more information and a deeper dive into how it works.

### Why "Synthesis"?

The Synthesis name dates back to the last major refactoring of the codebase. The idea was to take the existing object mapping framework and "synthesize" planned upgrades to it, along with adding some of the great ideas other publicly released mapping frameworks had come up with, into the mapping framework that would bring self-awareness to Sitecore and achieve world domination. Or at least a framework that would be fun to use and make your life easier ;)