Public MustInherit Class WorkbeatEntity
	Private m_data As Dto
	Public MustOverride Property data() As Dto
	Public MustOverride Property workbeatId As String
	Public MustOverride Property fechaLastUpdate As DateTime
	Public MustOverride Property active As Boolean
End Class


Public MustInherit Class Dto


	Public MustOverride Function Clone() As Dto


End Class