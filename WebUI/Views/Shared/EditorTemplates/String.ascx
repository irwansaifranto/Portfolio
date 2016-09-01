<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<%= Html.TextBox("", ViewData.TemplateInfo.FormattedModelValue, ViewData["htmlAttributes"]) %>