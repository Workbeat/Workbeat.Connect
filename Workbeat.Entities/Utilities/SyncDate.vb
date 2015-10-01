Namespace Utilities.Sync
	Public Class SyncDate

		Public Shared Function getEntityLastUpdate(entityType As EntityTypes, clientName As String) As DateTime
			Dim dal As New EntitySyncDateDal
			Dim fechaLastUpdate As DateTime

			fechaLastUpdate = dal.getEntityLastUpdate(clientName, entityType)

			Return fechaLastUpdate
		End Function


		Public Shared Sub setEntityLastUpdate(entityType As EntityTypes, clientName As String, fecha As DateTime)
			Dim dal As New EntitySyncDateDal
			dal.setEntityLastUpdate(clientName, entityType, fecha)
		End Sub

	End Class

End Namespace
