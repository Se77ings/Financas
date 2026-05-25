
"use client"
import { useRouter } from "next/navigation";
import EffectiveDateSummary from '@/models/EffectiveDate/EffectiveDateSummary';
import Content from '@/public/components/footer/Content';
import Home from '@/public/components/Home';
import { IonIcon } from '@ionic/react';
import { home, addOutline } from 'ionicons/icons';
import { useEffect, useState } from 'react';
import { getEffectiveDateListAsync } from '@/services/effectiveDateServices';

export default function Main() {





  return (
    <Home />
  );



  const onSetState = (value: any) => {
    console.log(value);
  }
  // create method to declare funct setstate



}