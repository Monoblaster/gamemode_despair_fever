//realistic health system
//blood and oxygen
//position based damage

function Player::SetBlood(%player,%tag,%percent)
{
    //blood is a precentage
    %player.SetTag("Blood",%tag,%percent);
    return %player;
}

function Player::GetBlood(%player)
{
    //blood is a precentage
    return mClamp(%player.GetTagTotal("Blood"),0,100);
}

function Player::SetOxygen(%player,%tag,%percent)
{
    //oxygen is a precentage
    %player.SetTag("Oxygen",%tag,%percent);
}

function Player::GetOxygen(%player)
{
    //blood is a precentage
    return mClamp(%player.GetTagTotal("Oxygen"),0,100);
}

function Player::SetDamageResistance(%player,%tag,%percent)
{
    //oxygen is a precentage
    %player.SetTag("DamageResistance",%tag,%percent);
}

function Player::SetDamageResistance(%player)
{
    //blood is a precentage
    return mClamp(%player.GetTagTotal("DamageResistance"),0,100);
}


function Damage_GetBodyPart(%player,%hitPos)
{
    if(!isObject(%player))
    {
        //no player
        return -1;
    }
    
    if(!(%player.getType() & $TypeMasks::PlayerObjectType))
    {
        //not player
        return -1;
    }

    %relHit = vectorSub(%hitPos,%player.getPosition());
    %x = getWord(%relHit,0);
    %y = getWord(%relHit,1);

    %forVec = %player.getForwardVector();
    %forY = getWord(%forVec,1);
    %rotation = mACos(vectorDot(%forVec,"1 0 0") / vectorLen(%forVec));
    talk(%rotation);
    if(%forY < 0)
    {
        %rotation = mAbs(%rotation - (2 * $PI));
    }

    %rotY = mCos(%rotation) * %x - mSin(%rotation) * %y; 
    %rotX = mSin(%rotation) * %x + mCos(%rotation) * %y; 
    %z = getWord(%relHit,2);

    if(%z < 1)
    {
        talk("Legs");
    }
    else if(%z < 1.95)
    {
        if(mAbs(%rotX) > 0.35)
        {
            if(%rotX > 0)
            {
                talk("rArm");
            }
            else
            {
                talk("lArm");
            }
            
        }
        else
        {
            talk("Body");
        }
    }
    else if(%z < 2.7)
    {
        talk("Head");
    }

    if(%z > 2.64)
    {
        talk("Top");
    }
    else if(mAbs(%rotX) > 0.6)
    {
        talk("Side");
    }
    else if(%rotY > 0)
    {
        talk("Back");
    }
    else
    {
        talk("Front");
    }
}