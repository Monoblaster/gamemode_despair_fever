function StatusEffects_Define(%name,%incompat,%remove)
{
    if(!isObject(StatusEffects))
    {
        %list = List_NewList();
        %list.setName(StatusEffects);
    }
    %list = StatusEffects;
    %list.add(%incompat TAB %remove,"",%name);
}

function StatusEffects_GetList(%player)
{
    if(!isObject(%player.StatusEffects))
    {
        %player.StatusEffects = List_NewList();
    }
    %list = %player.StatusEffects;

    return %list;
}

function Player::GiveStatusEffect(%player,%name,%value)
{
    %list = StatusEffects_GetList(%player);
    %list.add(%value, "", %name);
    return %player;
}

function Player::RemoveStatusEffect(%player,%name)
{
    %list = StatusEffects_GetList(%player);
    %list.remove(%list.getRow(%name));
    return %player;
}

function Player::HasStatusEffect(%player,%name)
{
    %list = StatusEffects_GetList(%player);
    return %list.IsTag(%name);
}