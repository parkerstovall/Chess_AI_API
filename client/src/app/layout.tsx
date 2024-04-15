import { Metadata } from "next";
import "../index.css";

export const metadata: Metadata = {
  title: "React App",
  description: "Web site created with Next.js.",
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en">
      <body>
        <div id="root">{children}</div>
      </body>
    </html>
  );
}
//The `content` option in your Tailwind CSS configuration is missing or empty.
//https://tailwindcss.com/docs/content-configuration
//https://nextjs.org/docs/basic-features/eslint#migrating-existing-config
