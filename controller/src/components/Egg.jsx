import React, { Component } from "react";

import egg from "../../assets/images/egg_1.png";

class Egg extends Component {
  render() {
    const { ready } = this.props;
    return (
      <div className="egg">
        <img
          src={egg}
          onClick={() => {
            ready();
          }}
        />
      </div>
    );
  }
}

export default Egg;
