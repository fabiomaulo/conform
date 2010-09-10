using ConfOrm.Shop.Inflectors;
using NUnit.Framework;

namespace ConfOrm.ShopTests.InflectorsTests
{
	[TestFixture]
	public class SpanishInflectorTest : CommonInflectorTests
	{
		public SpanishInflectorTest()
		{
			SingularToPlural.Add("ingl�s", "ingleses");
			SingularToPlural.Add("hijo", "hijos");
			SingularToPlural.Add("paz", "paces");
			SingularToPlural.Add("crisis", "crisis");
			SingularToPlural.Add("praxis", "praxis");
			SingularToPlural.Add("apendicitis", "apendicitis");
			SingularToPlural.Add("llave", "llaves");
			SingularToPlural.Add("auto", "autos");
			SingularToPlural.Add("ord�n", "ordenes");
			SingularToPlural.Add("item", "items");
			SingularToPlural.Add("linea", "lineas");
			SingularToPlural.Add("proveedor", "proveedores");
			SingularToPlural.Add("Terminal", "Terminales");
			SingularToPlural.Add("ParteFichaTecnica", "ParteFichaTecnicas");
			SingularToPlural.Add("pago", "pagos");
			SingularToPlural.Add("Ubicaci�n", "Ubicaciones");
			SingularToPlural.Add("Orig�n", "Origenes");
			SingularToPlural.Add("ciudad", "ciudades");
			SingularToPlural.Add("documento", "documentos");
			SingularToPlural.Add("Historial", "Historiales");
			SingularToPlural.Add("Promoci�n", "Promociones");

			SingularToPlural.Add("Ord�n", "Ordenes");
			SingularToPlural.Add("Cliente", "Clientes");
			SingularToPlural.Add("Proveedor", "Proveedores");
			SingularToPlural.Add("Factura", "Facturas");

			SingularToPlural.Add("Tarifa", "Tarifas");
			SingularToPlural.Add("Telefono", "Telefonos");
			SingularToPlural.Add("TelefonoTipo", "TelefonoTipos");
			SingularToPlural.Add("Territorio", "Territorios");
			SingularToPlural.Add("Titulo", "Titulos");
			SingularToPlural.Add("Tratamiento", "Tratamientos");
			SingularToPlural.Add("Usuario", "Usuarios");
			SingularToPlural.Add("Rol", "Roles");
			SingularToPlural.Add("Item", "Items");

			TestInflector = new SpanishInflector();
		}
	}
}