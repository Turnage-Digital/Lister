import React from "react";

const AccountMenu = () => {
  return (
    <button
      type="button"
      className="flex h-10 w-10 items-center justify-center rounded-full shadow-lg transition-colors hover:bg-[#003E6B]"
    >
      <span className="material-icons text-2xl text-white">account_circle</span>
    </button>
  );
};

export { AccountMenu };
