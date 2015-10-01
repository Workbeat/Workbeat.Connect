Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Workbeat.Entities.Utilities


<TestClass()> Public Class AObjectMapperTests

	<TestMethod()> Public Sub DoSaveTests()
		Dim objMap As ObjectMapper.Map
		objMap = ObjectMapper.Map.getMapFromWorkbeatId(Workbeat.Entities.EntityTypes.Empleado, 1, "test")
		objMap.externalId = 11
		objMap.Save()
		Assert.AreEqual("1", objMap.workbeatId, "Test 1:El workbeatId no es el correcto")
		Assert.AreEqual("11", objMap.externalId, "Test 1:El externalId no es el correcto")
		Assert.AreEqual("test", objMap.clientName, "Test 1:El clientName no es el correcto")
		Assert.AreEqual(Workbeat.Entities.EntityTypes.Empleado, objMap.entityType, "Test 1:El entity type no es el correcto")


		objMap = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromWorkbeatId(Workbeat.Entities.EntityTypes.Empleado, 1, "test")
		Assert.AreEqual("1", objMap.workbeatId, "Test 2:El workbeatId no es el correcto")
		Assert.AreEqual("11", objMap.externalId, "Test 2:El externalId no es el correcto")
		Assert.AreEqual("test", objMap.clientName, "Test 2:El clientName no es el correcto")
		Assert.AreEqual(Workbeat.Entities.EntityTypes.Empleado, objMap.entityType, "Test 2:El entity type no es el correcto")


		objMap = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromWorkbeatId(Workbeat.Entities.EntityTypes.Empleado, 1, "Nomipaq")
		Assert.AreEqual("Nomipaq", objMap.clientName, "Test 3:El clientName no es el correcto")
		Assert.AreEqual("1", objMap.workbeatId, "Test 3:El workbeatId no es el correcto")
		Assert.IsNull(objMap.externalId, "Test 3:El externalId no es el correcto")
		Assert.AreEqual(True, objMap.isNew, "Test 3:El mapeo debe ser nuevo")
		Assert.AreEqual(Workbeat.Entities.EntityTypes.Empleado, objMap.entityType, "Test 3:El entity type no es el correcto")


		' cambiar external Id

		objMap = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromWorkbeatId(Workbeat.Entities.EntityTypes.Empleado, 1, "test")
		objMap.externalId = 13
		objMap.Save()

		objMap = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromWorkbeatId(Workbeat.Entities.EntityTypes.Empleado, 1, "test")
		Assert.AreEqual("1", objMap.workbeatId, "Test 4:El workbeatId no es el correcto")
		Assert.AreEqual("13", objMap.externalId, "Test 4:El externalId no es el correcto")
		Assert.AreEqual("test", objMap.clientName, "Test 4:El clientName no es el correcto")
		Assert.AreEqual(Workbeat.Entities.EntityTypes.Empleado, objMap.entityType, "Test 4:El entity type no es el correcto")

	End Sub

	<TestMethod()> Public Sub getTests()

	End Sub


	<TestMethod()> Public Sub updateTests()

	End Sub


End Class