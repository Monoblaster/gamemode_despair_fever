datablock ItemData(RevolverItem)
{
	category = "DespairWeapon";
	classname = "DespairWeapon";

    shapeFile = $Despair::Path @ "res/shapes/weapons/revolver.dts";

    image = RevolverImage;
    mass = 1;
    drag = 0.3;
    density = 0.2;
    elasticity = 0;
    friction = 1;

	uiName = "Revolver";
	canDrop = true;

	doColorShift = true;
	colorShiftColor = "1 0.8 0 1";

	itemPropsClass = "GunProps";
	itemPropsAlways = true;

    maxAmmo = 6;
};

function GunProps::onAdd(%this)
{
	%this.ammo = 1;
    %this.maxAmmo = %this.sourceItemData.maxAmmo;
    %this.owner = "";
}

datablock ShapeBaseImageData(RevolverImage)
{
    className = "WeaponImage";
    item = RevolverItem;

	doColorShift = true;
	colorShiftColor = "1 0.8 0 1";

    shapeFile = $Despair::Path @ "res/shapes/weapons/revolver.dts";

    useCustomStates = true;

    bulletCount = 1;
    bulletSpread = 0.015;
    bulletSpeed = 200;
    
    type = "gun";
    fireThread = "activate";

    fireSound = "RevolverFireSound";
    fireSoundDistant = "RevolverFireSoundDistant";

    armReady = true;

    fireDelay = 0.7;
    fireManual = true;

    gun = true;

    damage = 100;
    penetrate = true;
};

function RevolverImage::onFire(%image, %obj)
{
    %parent = GenericBulletWeapon::onFire(%image, %obj);
    %props = %obj.getItemProps();
    if (isObject(%obj.client))
        commandToClient(%obj.client, 'CenterPrint', "<just:right><font:cambria:24>\c3" @ %props.ammo @ " bullets");

    if(%parent) //Spawn a bullet shell
    {
        %pos = %obj.getEyePoint();
        %end = VectorSub(%pos, getWords(VectorScale(%obj.getMuzzleVector(0), 5), 0, 1) SPC 10);

        %up = %obj.getUpVector();
        %forward = %obj.getForwardVector();
        %right = VectorCross(%forward, %up);

        %end = vectorAdd(%end, vectorScale(%right, 5));

        %ray = containerRayCast(%pos, %end, $SprayBloodMask); //Fibers fall

        if(%ray)
            %decal = spawnDecal(BulletShellShape, getWords(%ray, 1, 3), getWords(%ray, 4, 6), 1, "", "", "", 1); //noUnclutterCheck is true
    }
    return %parent;
}

function RevolverImage::onMount(%this, %obj, %slot)
{
	%props = %obj.getItemProps();
	if (isObject(%obj.client))
		commandToClient(%obj.client, 'CenterPrint', "<just:right><font:cambria:24>\c3" @ %props.ammo @ " bullets");
}

function RevolverImage::onUnMount(%this, %obj, %slot)
{
	%props = %obj.getItemProps();
	if (isObject(%obj.client))
		commandToClient(%obj.client, 'ClearCenterPrint');
}

function GenericBulletWeapon::onFire(%image, %player)
{
    %eyePoint = %player.getEyePoint();
    
    %props = %player.getItemProps();
    if (%props.ammo !$= "")
    {
        if (%props.ammo <= 0)
        {
            serverPlay3d("GunEmptySound", %player.getEyePoint());
            return false;
        }
        
        %props.ammo--;
    }
    
    %player.lastFireTime = $Sim::Time;

    %up = %player.getUpVector();
    %forward = %player.getForwardVector();
    %right = VectorCross(%forward, %up);

    %speed = 80;

    if (%image.bulletSpeed !$= "")
        %speed = %image.bulletSpeed;

    %damage = %image.damage;
    
    %spread = %image.bulletSpread;

    for (%i = 0; %i < %image.bulletCount; %i++)
    {
        %velocity = VectorScale(%player.getAimVector(), %speed);

        %yaw = (getRandom() * 2 - 1) * %image.bulletSpread;
        %pitch = (getRandom() * 2 - 1) * %image.bulletSpread * 0.5;

        if (%sprayTest)
        {
            %pitch += %recoilPitch;
            %yaw += %recoilYaw;
            %recoilAngle += mDegToRad((getRandom() * 2 - 1) * (45 * 0.5));
            // %angle = (getRandom() * 2 - 1) * 1.5709;
            %magnitude = mDegToRad(1);
            %recoilYaw += mCos(%recoilAngle) * %magnitude;
            %recoilPitch += mSin(%recoilAngle) * %magnitude;
            // %pitch -= (%sprayIndex * 0.02);
            // %yaw += 0.06 * mSin(%sprayIndex * 0.27) * (%sprayIndex / 7);
        }

        %velocity = MatrixMulVector("0 0 0 " @ %right SPC %pitch, %velocity);
        %velocity = MatrixMulVector("0 0 0 0 0 1" SPC %yaw, %velocity);

        %projectile = new Projectile()
        {
            datablock = FeverProjectile;
            sourceObject = %player;
            sourceSlot = 0;
            initialPosition = %eyePoint;
            initialVelocity = %velocity;
            damage = %damage;
            source = %player.client;
            image = %image;
            penetrate = %image.penetrate;
        };
    }

    %i = ClientGroup.getCount();

    while (%i-- >= 0)
    {
        %client = ClientGroup.getObject(%i);

        if (%client == %player.client)
            %client.play2D(%image.fireSound);
        else
        {
            %control = %client.getControlObject();
            if(isObject(%control))
            {
                %distance = vectorDist(%eyePoint, %control.getPosition());
                if (%image.fireSoundDistant !$= "" && %distance >= 50)
                    %client.play2D(%image.fireSoundDistant);
                else
                    %client.play3D(%image.fireSound, %eyePoint);
            }
        }
    }

    if (%image.fireThread !$= "")
        %player.playThread(2, %image.fireThread);

    if (%player.client)
        %player.client.updateBottomPrint();
	
	if(getRandom() < 0.3)
		%player.spawnFiber();
    return true;
}

datablock staticShapeData(BulletHoleShape)
{
	shapeFile = $Despair::Path @ "res/shapes/bullethole.dts";
	doColorShift = true;
	colorShiftColor = "0.25 0.25 0.25 1";
};

datablock StaticShapeData(BulletShellShape)
{
	shapeFile = $Despair::Path @ "res/shapes/357shell.dts";
	canClean = true;
};

datablock ExplosionData(QuietGunExplosion : GunExplosion)
{
    soundProfile = "";
};

datablock ProjectileData(FeverProjectile : GunProjectile)
{
    uiName = "";
    explosion = QuietGunExplosion;
};

function FeverProjectile::onCollision(%data, %projectile, %object, %fade, %position, %normal, %velocity)
{
    if (!(%object.getType() & $TypeMasks::PlayerObjectType))
    {
        serverPlay3d("ImpactEnvSound" @ getRandom(1,4), %position);
        %color = "0.25 0.25 0.25 1";
        if (%projectile.hitPlayer)
            %color = 0.4 + 0.2 * getRandom() @ " 0 0 1";

        spawnDecal(BulletHoleShape, VectorAdd(%position, VectorScale(%normal, 0.02)), %normal, 3, %color, "", "", 1); //noUnclutterCheck is true
        if(%object.getType() & $TypeMasks::FxBrickObjectType && %object.getDataBlock().isDoor)
        {
            %dam = %object.doorDamage(10);
            if(%dam)
            {
                ServerPlay3D(WoodHitSound, %position);
                return %dam;
            }
        }
        return;
    }

    if(%projectile.penetrate)
    {
        %source = %projectile.source;
        %damage = %projectile.damage;
        %image = %projectile.image;
        %projectile.delete();
        %projectile = new Projectile()
        {
            datablock = %data;
            sourceObject = %object;
            sourceSlot = 0;
            initialPosition = %position;
            initialVelocity = %velocity;
            damage = %damage;
            source = %source;
            image = %image;
            penetrate = 1;
            hitPlayer = true;
        };
    }

    serverPlay3d("ImpactFleshSound", %position);
    %object.damage(%projectile.source.player, %position, %projectile.damage, %projectile.image.type);
    sprayBloodFromHit(%position, VectorScale(%velocity, 0.2), %object);
}

datablock AudioProfile(RevolverFireSound)
{
    fileName = $Despair::Path @ "res/sounds/weapons/rev_fire.wav";
    description = AudioDefault3d;
    preload = true;
};

datablock AudioProfile(RevolverFireSoundDistant)
{
    fileName = $Despair::Path @ "res/sounds/weapons/rev_fire_distant.wav";
    description = Audio2d;
    preload = true;
};

datablock AudioProfile(ImpactEnvSound1)
{
    fileName = $Despair::Path @ "res/sounds/weapons/gun_miss1.wav";
    description = AudioDefault3d;
    preload = true;
};
datablock AudioProfile(ImpactEnvSound2)
{
    fileName = $Despair::Path @ "res/sounds/weapons/gun_miss2.wav";
    description = AudioDefault3d;
    preload = true;
};
datablock AudioProfile(ImpactEnvSound3)
{
    fileName = $Despair::Path @ "res/sounds/weapons/gun_miss3.wav";
    description = AudioDefault3d;
    preload = true;
};
datablock AudioProfile(ImpactEnvSound4)
{
    fileName = $Despair::Path @ "res/sounds/weapons/gun_miss4.wav";
    description = AudioDefault3d;
    preload = true;
};

datablock AudioProfile(ImpactFleshSound)
{
    fileName = $Despair::Path @ "res/sounds/weapons/flesh_bullet.wav";
    description = AudioDefault3d;
    preload = true;
};

datablock AudioProfile(GunEmptySound)
{
    fileName = $Despair::Path @ "res/sounds/weapons/gun_empty.wav";
    description = AudioDefault3d;
    preload = true;
};
