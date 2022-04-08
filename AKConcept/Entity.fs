module CremeEntity

open System


// Domain
type CremeEntity =
    | User
    | Recipe

type NotificationData = 
    { SourceId: string
      SourceType: CremeEntity
      TargetsId: string list
      TargetsType: CremeEntity }

type NotificationEvent =
    | Followed of NotificationData
    | Created of NotificationData
    | Updated of NotificationData
    | Cooked of NotificationData
    | Reacted of NotificationData
    | UserArchiveReady of NotificationData

// DTO
type CremeEvent<'T> =
   { id: string
     data: 'T }

type NotificationEventDTO =
    { Action: string
      SourceId: string
      SourceType: string
      TargetsId: string list
      TargetsType: string }
