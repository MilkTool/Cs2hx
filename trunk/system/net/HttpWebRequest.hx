package system.net;
import system.io.Stream;
import system.NotImplementedException;

class HttpWebRequest extends WebRequest
{

	public function new() 
	{
		super();
	}
	
	public static function Create(url:String):HttpWebRequest
	{
		return throw new NotImplementedException();
	}
	
	public var Method:String;
	public var ContentType:String;
	public var Timeout:Int;
	public var ContentLength:Int;
	
	public function GetRequestStream():Stream
	{
		return throw new NotImplementedException();
	}
	
	public function GetResponse():HttpWebResponse
	{
		return throw new NotImplementedException();
	}
	
}