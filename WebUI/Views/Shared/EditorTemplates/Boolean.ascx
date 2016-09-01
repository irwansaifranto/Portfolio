<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<script runat="server">
    private bool? Value
    {
        get
        {
            bool? value = null;
            if (ViewData.Model != null)
            {
                value = Convert.ToBoolean(ViewData.Model,
                                          System.Globalization.CultureInfo.InvariantCulture);
            }
            return value;
        }
    }
</script>

<div class="checkbox <%= ViewData["class"] %>">
    <label>
        <%= Html.CheckBox("", Value ?? false)%> <%= ViewData["text"] %>
    </label>
</div>