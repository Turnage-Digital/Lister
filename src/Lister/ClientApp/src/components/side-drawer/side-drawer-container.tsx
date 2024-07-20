import React, { PropsWithChildren } from "react";

type Props = PropsWithChildren;

const SideDrawerContainer = ({ children }: Props) => {
  return <>{children}</>;
};

export default SideDrawerContainer;
