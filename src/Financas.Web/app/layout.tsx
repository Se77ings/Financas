
import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";
import Footer from "@/public/components/footer/Footer";
import { Suspense } from "react";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "Finanças Pessoais",
  description: "Finanças Pessoais",
};
export default function RootLayout(
  { children, }: Readonly<{ children: React.ReactNode; }>
) {
  return (
    <html lang="en" className={`${geistSans.variable} ${geistMono.variable} h-full w-full antialiased`}>
      <body className="flex flex-col h-full w-full min-h-screen overflow-hidden text-gray-100 bg-blue-950">
        <main className="flex flex-1 flex-col min-h-0 w-full min-w-0 overflow-hidden">{children}</main>
        <Suspense fallback={<div className="h-11 bg-blue-950/20 border-t border-white/5" />}>
          <Footer />
        </Suspense>
      </body>
    </html>
  );
}