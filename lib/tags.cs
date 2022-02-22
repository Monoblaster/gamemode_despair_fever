function Tags_GetList(%obj)
{
    %tags = %obj.tags;
    if(!isObject(%tags))
    {
        %tags = List_NewList();
        %obj.tags = %tags;
    }
    return %tags;
}

function SimObject::SetTag(%obj,%type,%id,%value)
{
    %tag = %type @ "_" @ %id;
    %tags = Tags_GetList(%obj);
    %tags.total[%type] += %tags.total[%type] - %obj.GetTag(%type,%id) + %value;
    if(!%tags.isTag(%id))
    {
        %tags.add(%Value,"",%tag);
    }
    else
    {
        %tags.set(%tags.getRow(%tag),%value);
    }
    return %obj;
}

function SimObject::AddTag(%obj,%type,%id,%value)
{
    %tag = %type @ "_" @ %id;
    %tags = Tags_GetList(%obj,%type);
    %currValue = %tags.getValue(%tags.getRow(%tag));
    %newValue = %currValue + %value;
    return %obj.setTag(%type,%tag,%newValue);
}

function SimObject::GetTag(%obj,%type,%id)
{
    %tag = %type @ "_" @ %id;
    %tags = Tags_GetList(%obj);
    return %tags.getValue(%tags.getRow(%tag));
}

function SimObject::GetTagTotal(%obj,%type)
{
    %tags = Tags_GetList(%obj);
    return %tags.total[%type];
}