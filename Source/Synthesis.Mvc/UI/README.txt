These classes are designed to be wired to an IoC container of your choice to enable pretty sweet constructor injection into controller renderings.

The general idea is that you wire IRenderingContext up to SitecoreRenderingContext (and if you wish, IContextDatabase, et al as well to SitecoreRenderingContext).

You may do the wiring as a singleton as SitecoreRenderingContext has no internal state.

Then you can simply constructor inject your rendering context, e.g.

public class Foo : Controller
{
	private readonly IRenderingContext _renderingContext;

	public Foo(IRenderingContext renderingContext) 
	{
		Assert.ArgumentNotNull(renderingContext, nameof(renderingContext));

		_renderingContext = renderingContext;
	}

	public ActionResult DoFoo() 
	{
		var model = _renderingContext.GetRenderingDatasource<IExpectedTypeItem>();

		// TODO: check for null here, which occurs if the datasource is the wrong type or not set (return a different view or Content(string.Empty))

		return View(model);
	}
}

This buys you stupidly easy unit tests on your controller renderings (without FakeDb!) because now you can speak entirely in Synthesis interfaces which you can fake in your favorite idiom.

I like NSubstitute, myself.