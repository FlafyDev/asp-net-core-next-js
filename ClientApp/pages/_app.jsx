import React, { useEffect, useState, useContext } from 'react';
import { LoggedContext } from '../context/loggedContext';
import { OpenPopupContext } from '../components/Popup/context';
import '../styles/globals.css'
import Menu from '../components/Menu'
import styles from '../styles/App.module.css'
import useBackgroundTransitioner from '../hooks/useBackgroundTransitioner';
import Popup from '../components/Popup';

function MyApp({ Component, pageProps }) {
  const [backgroundElement, addBackground] = useBackgroundTransitioner(`linear-gradient(#e66465, #9198e5)`);
  const [loggedInfo, setLoggedInfo] = useState({ username: '', isAdmin: false });
  const [popupChildren, setPopupChildren] = useState(null);

  const requestLoggedInfo = async () => {
    const data = await fetch('api/Users/GetData').then(res => res.json()).catch(() => null);
    setLoggedInfo({ username: data?.username || '', isAdmin: data?.isAdmin || false });
  }

  useEffect(() => {
    requestLoggedInfo();
    const interval = setInterval(
      () => requestLoggedInfo(),
      10000
    );
    return () => {
      clearInterval(interval);
    }
  }, [])

  return (
    <div style={{ width: '100vw', height: '100vh' }}>
      {backgroundElement}
      <LoggedContext.Provider value={{ info: loggedInfo, requestLoggedInfo }}>
        <OpenPopupContext.Provider value={(children) => setPopupChildren(children)} >
          {
            !popupChildren || <Popup onClose={() => {
              setPopupChildren(null);
            }}>{popupChildren}</Popup>
          }
          <Menu />
          <div style={{ position: 'relative', top: '61px', height: 'Calc(100vh - 61px)' }}>
            <Component {...pageProps} addBackground={addBackground} />
          </div>
        </OpenPopupContext.Provider>
      </LoggedContext.Provider>
    </div >
  )
}

export default MyApp
