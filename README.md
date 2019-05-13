# Synthesis

Synthesis is an object mapping framework for Sitecore that enables developing more reliable and maintainable sites in less time than traditional Sitecore development. It is a strongly typed template object generator that is easily understandable for developers with either a Sitecore or traditional .NET background. It neatly integrates with Sitecore MVC (via the `Synthesis.Mvc` package) as a View rendering model provider and IoC dependency for controller renderings.

## How is Synthesis different from say, [Glass](http://glass.lu/) or [Fortis](http://fortis.ws/)?

Glass and Fortis both serve a similar goal - mapping database templates onto C# objects. However they both take different avenues to arrive there.

Glass is more of a traditional ORM tool, similar to say nHibernate for SQL. If you're looking to map your Sitecore templates onto pure POCO (Plain Ol' C# Object) classes that use primitive values (e.g. strings), then Glass may be your jam. Glass' claim to fame is that is does not internally wrap the Sitecore `Item` class, and actually does _map_ directly to C# object properties. In other words its model objects have zero ties to Sitecore once mapped.

Fortis works fairly similarly to Glass at a conceptual level, but unlike Glass (and like Synthesis) it is a _[wrapper](https://cardinalcore.co.uk/2015/06/15/life-through-a-lens-mappers-and-wrappers/)_. In other words, Fortis' objects are essentially facades over the Sitecore `Item` class and are internally tied to Sitecore's objects. This can result in improved performance over a mapper in some cases, although Glass has been pretty well optimized.

Both Glass and Fortis are capable of using code generated model classes, where some tool (e.g. [T4 templates](https://github.com/hermanussen/sitecore.codegenerator), [Transitus](http://fortis.ws/fortis-collection/transitus/), etc) reads either serialized or database templates and automatically generates a complete model based on what is actually stored in Sitecore. This can be a great time saver if you do not wish to have maniacal control over model code. However in both cases code generation is treated as a separate tool from the actual mapping tool.

Synthesis takes a different approach. Synthesis integrates the code generator with the mapping framework, which allows the model and framework code to be more harmonious. For example, Synthesis objects are always template type safe (you cannot map an item onto the wrong template class), cast compatible (if template B inherits template A, then you can cast any instance of B to an A using C# casting), and represent template inheritance with an interface hierarchy. Synthesis objects may be natively used as Content Search query models (even as interfaces). This integration also makes Synthesis ridiculously fast and reflection-free, because the mappings are all done with pregenerated code. It's also very easy to test code written against Synthesis, because everything is done through interfaces (even item axes and database operations). In most cases, it's not even required to use [FakeDb](https://github.com/sergeyshushlyapin/Sitecore.FakeDb).

Because of its integrated nature, Synthesis isn't really even an ORM like Glass and Fortis. Its design goal is not to map onto an arbitrary model, its design goal is to _represent the current state of the database in code_ and give the developer a way to safely code against that state knowing that the compiler will throw errors when, say, a template field is removed. It's more like "Sitecore template intellisense" than pure ORM. Does that preclude your using your own domain model? Absolutely not. But it does add additional safety to the data sources for your domain model.

What's the right tool for the job? That's about like [tabs vs spaces](https://www.youtube.com/watch?v=SsoOG6ZeyUI), there isn't one tool to rule them all. Weigh your options and pick the one that fits with your team's opinions and development style.

## I'm using modular architecture. Will Synthesis support models across projects?

Yes! Synthesis actually works very well with modular architecture (e.g. [Helix](http://helix.sitecore.net)/Habitat). Synthesis supports the idea of multiple _configurations_ that have different sets of included templates, excluded fields, and output generated code files. Configurations are registered in code, quite similarly to MVC Areas. To register a configuration (you'd want one per module):

1) Add a class to your module that derives from the `SynthesisConfigurationRegistration` abstract class. Required elements must be implemented, and some optional ones are overridable.
2) Add an instance of `SynthesisConfigRegistrar` to the `<initialize>` pipeline, and tell Synthesis to look for config registrations in your module assembl(ies), e.g.:

```xml
<!-- IMPORTANT: Each registrar instance must have a unique hint value for the patch to work correctly. -->
<processor type="Synthesis.Pipelines.Initialize.SynthesisConfigRegistrar, Synthesis" hint="MySite">
    <assemblies hint="list:AddAssembly">
        <framework>MySite.Framework</framework>
        <feature>MySite.Feature.*</feature>
    </assemblies>
</processor>
```

If you are convention-based about where templates belong for a module (like Helix is) then you can usually encode those conventions into a base registration class and implement even less code in each module registration. For an example of this, see the [Synthesis Habitat example implementation](https://github.com/kamsar/Habitat/tree/HabitatSynthesis/src/Foundation/Synthesis/code) which infers just about everything by convention.

Synthesis uses smart 'auto friending' across module configurations. For example if module B has a template "Bar" that inherits from "Foo" defined in module A, the generated interface for IBarItem will inherit from _the interface generated for the module A configuration_ instead of making something new. This keeps your models representing your architecture and makes lines of dependency between modules very obvious, even if the dependency is only at a template level. For this to work make sure you register your configurations from least to most specific (in Habitat terms, you'd register configurations for Foundation then Feature then Project).

## Show me the code! (How to do things in Synthesis)

```csharp
// convert a Sitecore.Data.Item to its Synthesis equivalent
var foo = Sitecore.Context.Item.As<ISampleItemItem>();

// get experience editor values
var pageEditor = foo.Title.RenderedValue;

// full metadata and traversal support
var parent = foo.Axes.Parent; // and this parent (and other axes results) is also a Synthesis object
var modified = foo.Statistics.Updated;

// add a new subitem with generics
var editing = foo.Add<ISampleItem>("Hello world");

// set text field values (automatically enters editing if not already there)
foo.Field.RawValue = "New value";

// set field values in a batch
foo.Editing.BeginEdit();
foo.Field.RawValue = "new value";
foo.DateField.Value = DateTime.Today;
foo.Editing.EndEdit();

// cast items to their base interfaces at will
var standardTemplate = (IStandardTemplateItem) foo;

// you can also convert items without specifying an output type (the instance will be the most specific template type available)
var generic = Sitecore.Context.Item.AsStronglyTyped();

var type = generic.GetType().FullName; // this will be a "SampleItem" concrete instance, which : ISampleItem, which : IStandardTemplateItem

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

    // if you have the title field indexed with value, this will grab the value out of Solr without any database work
    var solrString = exampleResult.Title.RawValue;

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

    // if you have the title field indexed with value, this will grab the value out of Solr without any database work
    var solrString = exampleResult.Title.RawValue;

    // but this value isn't stored in the index. Accessing it will transparently cause the Sitecore.Data.Item to be loaded, and the value retrieved. Nice huh?
    var promoted = exampleResult.Text.ExpandedLinksValue;
}

// use IoC to provide data sources to MVC controller renderings
// 1) map IRenderingContext to SitecoreRenderingContext in your IoC container
// 2) ...
public class FooController : Controller
{
    private readonly IRenderingContext _renderingContext;

    public FooController(IRenderingContext renderingContext) 
    {
        _renderingContext = renderingContext;
    }

    public ActionResult Foo()
    {
        var dataSource = _renderingContext.GetRenderingDatasource<IExpectedTypeItem>();

        if(dataSource == null)
        {
            // no datasource set, or datasource is wrong template type (or context item, if no datasource set)
        return Content("Derp.");
        }

        var model = new FooViewModel(dataSource);

        // set other model props here

        // Note that none of this controller directly used Sitecore APIs and thus does not require FakeDb nor HTTP context
        // to have unit tests written against it.

        return View(model);
    }
}

// use Synthesis.Mvc to provide Synthesis types as models to view renderings automatically
@model IMyExpectedTemplateItem

@Html.TextFor(m => m.SomeTextField)
```

Ready to try it? Get the package off NuGet and have fun! Installing the NuGet package will show a README in Visual Studio to help you get set up. See the docs in the [wiki](https://github.com/kamsar/Synthesis/wiki) for more information and a deeper dive into how it works.