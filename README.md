# dotmailer for Saleslogix / Infor CRM

This repository contains a connector that integrates dotmailer with Saleslogix (now Infor CRM). Originally built for us by a third party, it's used by quite a few customers. Please note that we don't offer support for this connector at this time, though we'd be happy to refer you to a number of third parties who are very capable of offering support and integration consultancy.

Some notes on the code:

* It currently relies on two third party components without source available:
  * dotmailer.sdk - this package was originally built by an internal team, when we only had a single, SOAP API. Now that we have much more advanced SOAP and REST APIs available, we have deprecated this SDK. A future version of this connector might benefit from a lightweight wrapper, or just direct API calls. Also, the dependency on the SDK prevents users in any dotmailer region other than region 1 from using this connector.
  * QGate.Components.Serialization.dll - QGate helped build this connector originally, and used this serialization library to load and save the configuration file for the connector. It would be possible to simple swap this out, but of course it would then render older configuration files unreadable.
* This connector was originally developed using .NET 4. However, it's been updated to target the .NET 4.5.2 framework as that's Microsoft's minimum supported version of the 4.x release as of January 2016.
* This code has been extensively cleaned to prepare it for public release. Since we no longer maintain a test Saleslogix environment, it is possible that there have been breaking changes or omissions. **We strongly encourage pull requests to fix any problems you encounter**.

This connector is released under the MIT license. Please see LICENSE.md for details.
