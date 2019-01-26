import React, { Component } from "react";

import egg from "../../assets/images/egg_1.png";

class Egg extends Component {
  constructor(props) {
    super(props);

    this.state = {
      taps: 0,
      cracks: 0,
      wobbling: false
    };
  }
  render() {
    const { ready } = this.props;
    const { taps, wobbling, cracks } = this.state;

    return (
      <div className="egg">
        <img
          src={egg}
          onClick={() => {
            this.setState(prevState => ({
              wobbling: true,
              taps: prevState.taps + 1
            }));
          }}
          className={wobbling ? "wobble" : ""}
          onAnimationEnd={() => {
            this.setState({
              wobbling: false,
              cracks: cracks + 1
            });

            if (this.state.cracks >= 2) {
              ready();
            }
          }}
        />
      </div>
    );
  }
}

export default Egg;
