import React from 'react';
import Header from './Header';
import Footer from './Footer';
import { observer } from 'mobx-react';

const Layout = ({ user, onLogout, children }) => {
  return (
    <div>
      <Header user={user} onLogout={onLogout} />
      <main>{children}</main>
      <Footer/>
    </div>
  );
};
export default observer(Layout);