import { NextResponse } from "next/server";
import { NextRequest } from "next/server";

export async function middleware(request: NextRequest) {
  const config: RequestInit = {
    method: request.method,
    body: request.body,
  };

  if (process.env.NODE_ENV === "development") {
    request.headers.append("Access-Control-Allow-Origin:", "localhost:5000");
    config.mode = "cors";
    config.credentials = "include";
  }

  let path = new URL(request.url).pathname.replace("/app", "");
  if (path.endsWith("/")) {
    path = path.slice(0, -1);
  }

  if (path.startsWith("/")) {
    path = path.slice(1);
  }

  config.headers = request.headers;
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

export const config = {
  matcher: "/app/api/:path*",
};
