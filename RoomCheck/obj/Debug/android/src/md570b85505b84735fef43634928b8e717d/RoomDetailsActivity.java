package md570b85505b84735fef43634928b8e717d;


public class RoomDetailsActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("RoomCheck.RoomDetailsActivity, RoomCheck, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", RoomDetailsActivity.class, __md_methods);
	}


	public RoomDetailsActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == RoomDetailsActivity.class)
			mono.android.TypeManager.Activate ("RoomCheck.RoomDetailsActivity, RoomCheck, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
