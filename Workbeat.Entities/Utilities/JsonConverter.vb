'Imports System.Web.Helpers
Imports System.Reflection
'Imports System.Dynamic
Imports System.Globalization
Imports System.Threading
Imports System.Runtime.Serialization
Imports System.Web.Script.Serialization
Imports System.IO
Imports System.Text

Namespace Utilities
	Public Class JsonConverter

		Private Shared ReadOnly log As log4net.ILog = log4net.LogManager.GetLogger(GetType(JsonConverter))

		Public Shared Function getJsonObj(ByVal obj As Object) As String
			Dim jsonString As String = ""

			Dim serializer As Json.DataContractJsonSerializer = New System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType())
			Dim MS As MemoryStream = New MemoryStream()
			serializer.WriteObject(MS, obj)
			jsonString = Encoding.UTF8.GetString(MS.ToArray())

			Return jsonString
		End Function

		Public Shared Function getObject(ByVal jsonString As String, ByVal obj As Object) As Object
			Dim objJson As New Object
			Dim MS As MemoryStream = New MemoryStream(Encoding.Unicode.GetBytes(jsonString))
			Dim serializer As Json.DataContractJsonSerializer = New Json.DataContractJsonSerializer(obj.GetType())
			objJson = serializer.ReadObject(MS)
			MS.Close()

			Return objJson
		End Function


		'Convierte la fecha en un String con el formato requerido
		Public Shared Function formatDateToString(ByVal fecha As DateTime) As String
			Dim fechaCadena As String = ""
			Thread.CurrentThread.CurrentCulture = New CultureInfo("en-US")
			fechaCadena = fecha.ToString("ddd, dd MMM yyyy HH:mm:ss") + " GMT"

			Return fechaCadena
		End Function

	End Class


End Namespace