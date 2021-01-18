# Quick start for building a Vonk Facade

[Vonk FHIR Server](http://fire.ly/vonk) can be used as a FHIR interface to your own data store. To make this happen, you can implement the two interfaces that let the server communicate with the data store.

This project contains an example on how to do this for a fictional relational database. It is meant to accompany the exercise on how to build a Vonk FHIR Facade on the [Vonk documentation](https://docs.fire.ly/firelyserver/getting_started.html).

|Develop|Master|
|---|---|
|[![Build Status](https://firely.visualstudio.com/Vonk.IdentityServer.Test/_apis/build/status/FirelyTeam.Vonk.IdentityServer.Test?branchName=master)](https://firely.visualstudio.com/Vonk.Facade.Starter/_build/latest?definitionId=27&branchName=master)|[![Build Status](https://firely.visualstudio.com/Vonk.IdentityServer.Test/_apis/build/status/FirelyTeam.Vonk.IdentityServer.Test?branchName=develop)](https://firely.visualstudio.com/Vonk.Facade.Starter/_build/latest?definitionId=27&branchName=develop)

## Get Vonk FHIR Server

Your implementation of a facade runs as a plugin to Vonk FHIR Server. Check the [Getting Started](http://docs.fire.ly/vonk/start.html) on how to obtain the binaries and an evaluation license.
