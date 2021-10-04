using Bindito.Core;
using Bindito.Unity;

namespace VeVantZeData.Collector
{
	public class VeVantZeDataConfigurator : PrefabConfigurator
	{
		public override void Configure(IContainerDefinition containerDefinition)
		{
			containerDefinition.Bind<Playthrough>().AsSingleton();
		}
	}
}
