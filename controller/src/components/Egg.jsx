import React, { Component } from "react";

import egg from "../../assets/images/egg_1.png";

class Egg extends Component {
  render() {
    return (
      <div className="egg">
        <img src={egg} />
      </div>
    );
  }
}

export default Egg;
