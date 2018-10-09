using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoutesManager : MonoBehaviour {
	public class RouteNode{
		public int order;
		public Vector3 position;
		public RouteNode(int ord,Vector3 pos){
			order=ord;
			position=pos;
		}
	}
	public class Route{
		public int routeId;
		public List<RouteNode> nodes;
		public Route(int id){
			routeId=id;
			nodes=new List<RouteNode>();
		}
	}
	List<Route> routes;
	// Use this for initialization
	void Start () {
		routes=new List<Route>();
		routes.Add(new Route(1));
        for(int i=0;i<20;i++)
        {
            int xOffset=(i%4)*10;
            int zOffset=(i/4)*15;
            routes[routes.Count - 1].nodes.Add(new RouteNode(i+1, new Vector3(xOffset, 0, zOffset)));
        }
		//routes[routes.Count-1].nodes.Add(new RouteNode(1,new Vector3(-7,0,0)));
		//routes[routes.Count-1].nodes.Add(new RouteNode(2,new Vector3(0,0,7)));
		//routes[routes.Count-1].nodes.Add(new RouteNode(3,new Vector3(7,0,0)));
		//routes[routes.Count-1].nodes.Add(new RouteNode(4,new Vector3(0,0,-7)));
	}
	public Route GetRoute(int id){
		for(int i=0;i<routes.Count;i++){
			if(routes[i].routeId==id)
				return routes[i];
		}
		return null;
	}
	// Update is called once per frame
	void Update () {
	
	}
}
