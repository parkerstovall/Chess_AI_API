import { NextResponse } from "next/server";
import { NextRequest } from "next/server";

// This function can be marked `async` if using `await` inside
export async function middleware(request: NextRequest) {
  if (process.env.NODE_ENV === "development") {
    request.headers.append("Access-Control-Allow-Origin:", "localhost:5000");

    const config: RequestInit = {
      method: request.method,
      mode: "cors",
      credentials: "include",
      headers: request.headers,
    };
    let path = new URL(request.url).pathname;
    if (path.endsWith("/")) {
      path = path.slice(0, -1);
    }

    if (path.startsWith("/")) {
      path = path.slice(1);
    }

    const params = new URL(request.url).search;
    const url = `${process.env.NEXT_PUBLIC_API_URL}/${path}/${params}`;
    const response = await fetch(url, config);

    if (response.status !== 200) {
      var text = await response.text();

      return NextResponse.json({ error: text }, { status: response.status });
    }

    return new Response(JSON.stringify(await response.json()), {
      status: 200,
      headers: response.headers,
    });
  }

  //return NextResponse.next();
}

// See "Matching Paths" below to learn more
export const config = {
  matcher: "/api/:path*",
};
