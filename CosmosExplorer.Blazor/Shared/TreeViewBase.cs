using Microsoft.AspNetCore.Components;
namespace CosmosExplorer.Blazor;

public partial class TreeViewBase<TValue> : ComponentBase
{
    protected List<TValue> AllItems;
    protected Dictionary<int, bool> _caretDown = new Dictionary<int, bool>();
    protected Dictionary<int, string> _caretcss = new Dictionary<int, string>();
    protected Dictionary<int, string> _nestedcss = new Dictionary<int, string>();


    [Parameter]
    public List<TValue> DataSource { get; set; }
    [Parameter]
    public string Index { get; set; }
    [Parameter]
    public string ParentId { get; set; }
    [Parameter]
    public string HasChildren { get; set; }
    [Parameter]
    public string Text { get; set; }
    [Parameter]
    public string Expanded { get; set; }
    [Parameter]
    public EventCallback<int> OnSelectItem { get; set; }
    [Parameter]
    public int? DefaultItem { get; set; }

    protected override Task OnInitializedAsync()
    {
        //asigning to its new instance to avoid exceptions.
        AllItems = DataSource.ToArray().ToList();

        if (AllItems != null)
        {
            foreach (var item in AllItems)
            {
                var _id = Convert.ToInt32(GetPropertyValue(item, Index));

                //initializing fields with default value.
                _caretDown.Add(_id, true);
                _caretcss.Add(_id, "caret");
                _nestedcss.Add(_id, "nested");
            }

        }
        if (DefaultItem is not null)
        {
            _caretcss[DefaultItem.Value] = "activeLink";
            _caretcss[1] = _caretDown[1] ? "caret caret-down" : "caret";
            _nestedcss[1] = _caretDown[1] ? "active" : "nested";
        }

        return base.OnInitializedAsync();
    }


    protected override Task OnParametersSetAsync()
    {
        //This will check if the first item in the
        // list/collection has a "parentId" then remove the "parentId" from it. 
        //Because we use the first item as a reference in the razor file, so it must not have "parentId".

        var Parem = AllItems.First();
        switch (GetPropertyType(Parem, ParentId))
        {
            case "Int32":
                if (Convert.ToInt32(GetPropertyValue(Parem, ParentId)) > 0)
                {
                    SetPropertyValue<int>(Parem, ParentId, 0);
                }
                break;
            case "String":
                if (GetPropertyValue(Parem, ParentId) != "")
                {
                    SetPropertyValue<string>(Parem, ParentId, "");
                }

                break;
            default:
                break;
        }

        return base.OnParametersSetAsync();
    }



    protected void SpanToggle(TValue item)
    {
        var _clckdItemid = Convert.ToInt32(GetPropertyValue(item, Index));

        _caretcss[_clckdItemid] = _caretDown[_clckdItemid] ? "caret caret-down" : "caret";
        _nestedcss[_clckdItemid] = _caretDown[_clckdItemid] ? "active" : "nested";
        
    }
    protected async Task Invoke(TValue item)
    {
        var _clckdItemid = Convert.ToInt32(GetPropertyValue(item, Index));


        foreach (var it in AllItems)
        {
            var clckdItemid = Convert.ToInt32(GetPropertyValue(it, Index));
            _caretcss[clckdItemid] = "";

        }
        _caretcss[_clckdItemid] = "activeLink";

        await OnSelectItem.InvokeAsync(_clckdItemid);

    }

    #region reflection methodes to get your property type, propert value and also set property value 
    protected string GetPropertyValue(TValue item, string Property)
    {

        if (item != null)
        {
            return item.GetType().GetProperty(Property).GetValue(item, null).ToString();
        }
        return "";

    }

    protected void SetPropertyValue<T>(TValue item, string Property, T value)
    {
        if (item != null)
        {
            item.GetType().GetProperty(Property).SetValue(item, value);
        }
    }

    protected string GetPropertyType(TValue item, string Property)
    {

        if (item != null)
        {
            return item.GetType().GetProperty(Property).PropertyType.Name;

        }
        return null;
    }
    #endregion
}